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
            var result = await _reservationRepository.GetReservations();

            return result.Map(ReservationDTO.MapFromList);
        }

        public async Task<Either<IError, List<ReservationDTO>>> GetUserReservations(string userId)
        {
            var result = await _reservationRepository.GetUserReservations(userId);

            return result.Map(ReservationDTO.MapFromList);
        }

        public async Task<Either<IError, List<ReservationDTO>>> GetReservationsByStartDate(DateTime startDate)
        {
            var result = await _reservationRepository.GetReservationsByStartDate(startDate);

            return result.Map(ReservationDTO.MapFromList);
        }

        public async Task<Either<IError, List<ReservationDTO>>> GetReservationsByEndDate(DateTime endDate)
        {
            var result = await _reservationRepository.GetReservationsByEndDate(endDate);

            return result.Map(ReservationDTO.MapFromList);
        }

        public async Task<Either<IError, List<ReservationDTO>>> GetReservationsByProductId(Guid productId)
        {
            var result = await _reservationRepository.GetReservationsByProductId(productId);

            return result.Map(ReservationDTO.MapFromList);
        }

        public async Task<Either<IError, ReservationDTO>> ExecuteReservationAction(Guid reservationId, ReservationAction action)
        {
            var reservation = await _reservationRepository.GetReservationByReservationId(reservationId);

            if (reservation.GetType() == typeof(IError))
            {
                return reservation.Map(ReservationDTO.MapFromModel);
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

            var result = await _reservationRepository.UpdateReservation(reservationToChange);

            return result.Map(ReservationDTO.MapFromModel);
        }

        public async Task<Either<IError, ReservationDTO>> ReserveProduct(ReservationDTO ReservationDTO, AppUser user)
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
                RenterId = user.Id,
                RenterName = user.Name,
                RenterEmail = user.Email,
                StartDate = ReservationDTO.StartDate,
                EndDate = ReservationDTO.EndDate,
            };

            var product = productResult.ValueUnsafe();
            if (product != null && product.RequiresApproval && product.Id != Guid.Empty)
            {
                reservation.IsApproved = false;
            }

            var createResult = await _reservationRepository.CreateReservation(reservation);

            return createResult.Map(ReservationDTO.MapFromModel);
        }

        public async Task<Either<IError, Reservation>> EditReservation(ReservationEditDTO reservationEditDTO)
        {
            var reservation = await _reservationRepository.GetReservationByReservationId(reservationEditDTO.ReservationId);
            if (reservation.IsNull())
            {
                return AppErrors.FailedToFindReservation;
            }

            ReservationDTO reservationDTO = new()
            {
                ProductId = reservationEditDTO.ProductId,
                RenterId = reservationEditDTO.RenterId,
                StartDate = reservationEditDTO.StartDate,
                EndDate = reservationEditDTO.EndDate,
            };

            var result = await ValidateReservationData(reservationDTO);
            if (result.IsSome)
            {
                return result.ValueUnsafe();
            }

            var editedReservation = new Reservation()
            {
                Id = reservationEditDTO.ReservationId,
                ProductId = reservationEditDTO.ProductId,
                RenterId = reservationEditDTO.RenterId,
                StartDate = reservationEditDTO.StartDate,
                EndDate = reservationEditDTO.EndDate,
            };

            return await _reservationRepository.EditReservation(editedReservation);
        }

        private async Task<Option<IError>> ValidateReservationData(ReservationDTO reservationDTO)
        {
            var now = _timeProvider.GetDateTimeNow();
            if (reservationDTO.ProductId == Guid.Empty)
            {
                return AppErrors.ProductNotFoundError;
            }

            if (reservationDTO.StartDate.Date < now)
            {
                return AppErrors.InvalidStartDate;
            }

            if (reservationDTO.EndDate.Date < now)
            {
                return AppErrors.InvalidEndDate;
            }

            if (reservationDTO.EndDate < reservationDTO.StartDate)
            {
                return AppErrors.EndDateBeforeStartDate;
            }

            if (reservationDTO.StartDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                return AppErrors.StartDateInWeekend;
            }

            if (reservationDTO.EndDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                return AppErrors.EndDateInWeekend;
            }

            if (ExceedsDayLimit(reservationDTO))
            {
                return AppErrors.ReservationIsTooLong;
            }

            if (await HasOverlappingReservation(reservationDTO))
            {
                return AppErrors.ReservationIsOverlapping;
            }

            var productResult = await _productRepository.GetProductById(reservationDTO.ProductId);

            if (productResult.IsNone)
            {
                return AppErrors.ProductDoesNotExist;
            }

            return Option<IError>.None;
        }

        private async Task<bool> HasOverlappingReservation(ReservationDTO dto)
        {
            var result = await _reservationRepository.GetOverlappingReservation(dto.Id, dto.ProductId, dto.StartDate, dto.EndDate);

            return result.IsRight;
        }

        private bool ExceedsDayLimit(ReservationDTO reservationDTO)
        {
            // Weekend days don't count toward the limit
            var weekendDays = AmountOfWeekendDays(reservationDTO.StartDate, reservationDTO.EndDate);

            var totalDays = (reservationDTO.EndDate - reservationDTO.StartDate).TotalDays - weekendDays;
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
