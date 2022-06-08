﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACI.Reservations.DBContext;
using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using ACI.Reservations.Repositories.Interfaces;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace ACI.Reservations.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ReservationDBContext _dbContext;

        public ReservationRepository(ReservationDBContext reservationDBContext)
        {
            _dbContext = reservationDBContext;
        }

        public async Task<Either<IError, List<Reservation>>> GetReservations()
        {
            var result = await _dbContext.Reservations.Include(x => x.Product).ToListAsync();

            if (result.Count <= 0)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result;
        }

        public async Task<Either<IError, List<Reservation>>> GetUserReservations(string userId)
        {
            var result = await _dbContext.Reservations.Include(x => x.Product).Where(x => x.RenterId == userId).ToListAsync();

            if (result.Count <= 0)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result;
        }

        public async Task<Either<IError, List<Reservation>>> GetReservationsByStartDate(DateTime startDate)
        {
            var result = await _dbContext.Reservations.Include(x => x.Product).Where(x => x.StartDate.Date == startDate.Date).ToListAsync();

            if (result.Count <= 0)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result;
        }

        public async Task<Either<IError, List<Reservation>>> GetReservationsByEndDate(DateTime endDate)
        {
            var result = await _dbContext.Reservations.Where(x => x.EndDate.Date == endDate.Date).ToListAsync();

            if (result.Count <= 0)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result;
        }

        public async Task<Either<IError, List<Reservation>>> GetReservationsByProductId(Guid productId)
        {
            var result = await _dbContext.Reservations.Where(x => x.ProductId == productId).ToListAsync();

            if (result.Count <= 0)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result;
        }

        public async Task<Either<IError, Reservation>> GetReservationByReservationId(Guid reservationId)
        {
            var result = await _dbContext.Reservations.Where(x => x.Id == reservationId).FirstOrDefaultAsync();

            if (result == null)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result;
        }

        public async Task<Either<IError, Reservation>> GetOverlappingReservation(Guid reservationId, Guid productId, DateTime startDate, DateTime endDate)
        {
            var result = await _dbContext.Reservations.Where(x => x.ProductId == productId && x.StartDate <= endDate && startDate < x.EndDate && x.Cancelled == false && x.Id != reservationId).FirstOrDefaultAsync();

            if (result == null)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result;
        }

        public async Task<Either<IError, Reservation>> UpdateReservation(Reservation reservation)
        {
            var reservationToUpdate = await _dbContext.Reservations.Where(x => x.Id == reservation.Id).FirstOrDefaultAsync();

            if (reservationToUpdate == null)
            {
                return AppErrors.FailedToFindReservation;
            }

            reservationToUpdate = reservation;

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return reservationToUpdate;
            }

            return AppErrors.FailedToSaveReservation;
        }

        public async Task<Either<IError, Reservation>> CreateReservation(Reservation reservation)
        {
            await _dbContext.Reservations.AddAsync(reservation);
            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return reservation;
            }

            return AppErrors.FailedToSaveReservation;
        }

        public async Task<Either<IError, Reservation>> EditReservation(Reservation reservation)
        {
            var retrievedreservation = await _dbContext.Reservations.Where(x => x.Id == reservation.Id).FirstOrDefaultAsync();

            if (retrievedreservation == null)
            {
                return AppErrors.FailedToFindReservation;
            }

            retrievedreservation.StartDate = reservation.StartDate;
            retrievedreservation.EndDate = reservation.EndDate;

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return retrievedreservation;
            }

            return AppErrors.FailedToSaveReservation;
        }
    }
}
