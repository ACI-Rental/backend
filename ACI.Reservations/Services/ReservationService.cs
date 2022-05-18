﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Reservations.Services.Interfaces;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.Extensions.Options;

namespace ACI.Reservations.Services
{
    public class ReservationService : IReservationService
    {
        private const int MaxReservationDays = 5;

        private readonly IReservationRepository _reservationRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITimeProvider _timeProvider;

        public ReservationService(IReservationRepository reservationRepository, IOptions<AppConfig> options, ITimeProvider timeProvider, IProductRepository productRepository)
        {
            _reservationRepository = reservationRepository;
            _timeProvider = timeProvider;
            _productRepository = productRepository;
        }

        public async Task<Either<IError, List<ReservationDTO>>> GetReservations()
        {
            return await _reservationRepository.GetReservations();
        }

        public async Task<Either<IError, List<ReservationDTO>>> GetReservationsByStartDate(DateTime startDate)
        {
            return await _reservationRepository.GetReservationsByStartDate(startDate);
        }

        public async Task<Either<IError, List<ReservationDTO>>> GetReservationsByEndDate(DateTime endDate)
        {
            return await _reservationRepository.GetReservationsByEndDate(endDate);
        }

        public async Task<Either<IError, List<ReservationDTO>>> GetReservationsByProductId(Guid productId)
        {
            return await _reservationRepository.GetReservationsByProductId(productId);
        }

        public async Task<Either<IError, ReservationDTO>> ExecuteReservationAction(Guid reservationId, ReservationAction action)
        {
            var reservation = await _reservationRepository.GetReservationByReservationId(reservationId);

            if (reservation.GetType() == typeof(IError))
            {
                return reservation;
            }

            var reservationToChange = reservation.ValueUnsafe();

            switch (action)
            {
                case ReservationAction.CANCEL:
                    reservationToChange.Cancelled = true;
                    break;
                case ReservationAction.PICKUP:
                    reservationToChange.PickedUpDate = DateTime.Now;
                    break;
                case ReservationAction.RETURN:
                    reservationToChange.ReturnDate = DateTime.Now;
                    break;
                default:
                    return AppErrors.InvalidReservationAction;
            }

            return await _reservationRepository.UpdateReservation(reservationToChange);
        }

        public async Task<Either<IError, ReservationDTO>> ReserveProduct(ReservationDTO ReservationDTO)
        {
            var result = await ValidateReservationData(ReservationDTO);
            if (result.IsSome)
            {
                return result.ValueUnsafe();
            }

            var productResult = await _productRepository.GetProductById(ReservationDTO.ProductId);
            if (productResult.IsNone)
            {
                return AppErrors.ProductNotFoundError;
            }

            var reservation = new Reservation()
            {
                ProductId = ReservationDTO.ProductId,
                RenterId = ReservationDTO.RenterId,
                StartDate = ReservationDTO.StartDate,
                EndDate = ReservationDTO.EndDate,
            };

            var product = productResult.ValueUnsafe();
            if (product != null && product.RequiresApproval && product.Id != Guid.Empty)
            {
                reservation.IsApproved = false;
            }

            return await _reservationRepository.CreateReservation(reservation);
        }

        private async Task<Option<IError>> ValidateReservationData(ReservationDTO ReservationDTO)
        {
            var now = _timeProvider.GetDateTimeNow();
            if (ReservationDTO.ProductId == Guid.Empty)
            {
                return AppErrors.ProductNotFoundError;
            }

            if (ReservationDTO.StartDate.Date < now)
            {
                return AppErrors.InvalidStartDate;
            }

            if (ReservationDTO.EndDate.Date < now)
            {
                return AppErrors.InvalidEndDate;
            }

            if (ReservationDTO.EndDate < ReservationDTO.StartDate)
            {
                return AppErrors.EndDateBeforeStartDate;
            }

            if (ReservationDTO.StartDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                return AppErrors.StartDateInWeekend;
            }

            if (ReservationDTO.EndDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                return AppErrors.EndDateInWeekend;
            }

            if (ExceedsDayLimit(ReservationDTO))
            {
                return AppErrors.ReservationIsTooLong;
            }

            if (await HasOverlappingReservation(ReservationDTO))
            {
                return AppErrors.ReservationIsOverlapping;
            }

            var productResult = await _productRepository.GetProductById(ReservationDTO.ProductId);

            if (productResult.IsNone)
            {
                return AppErrors.ProductDoesNotExist;
            }

            return Option<IError>.None;
        }

        private async Task<bool> HasOverlappingReservation(ReservationDTO dto)
        {
            var result = await _reservationRepository.GetOverlappingReservation(dto.ProductId, dto.StartDate, dto.EndDate);

            return result.IsRight;
        }

        private bool ExceedsDayLimit(ReservationDTO ReservationDTO)
        {
            // Weekend days don't count toward the limit
            var weekendDays = AmountOfWeekendDays(ReservationDTO.StartDate, ReservationDTO.EndDate);

            var totalDays = (ReservationDTO.EndDate - ReservationDTO.StartDate).TotalDays - weekendDays;
            return totalDays > MaxReservationDays;
        }

        private int AmountOfWeekendDays(DateTime startDate, DateTime endDate)
        {
            int amountOfWeekendDays = 0;
            for (DateTime i = startDate; i < endDate; i = i.AddDays(1))
            {
                if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
                {
                    amountOfWeekendDays++;
                }
            }

            return amountOfWeekendDays;
        }
    }
}
