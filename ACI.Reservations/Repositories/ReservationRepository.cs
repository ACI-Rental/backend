using ACI.Reservations.DBContext;
using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Repositories.Interfaces;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
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
            var result = await _dbContext.Reservations.ToListAsync() ?? Option<List<Reservation>>.None;

            if (result == Option<List<Reservation>>.None)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result.ValueUnsafe();
        }

        public async Task<Either<IError, List<Reservation>>> GetReservationsByStartDate(DateTime startDate)
        {
            var result = await _dbContext.Reservations.Where(x => x.StartDate.Date == startDate.Date).ToListAsync() ?? Option<List<Reservation>>.None;

            if (result == Option<List<Reservation>>.None)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result.ValueUnsafe();
        }

        public async Task<Either<IError, List<Reservation>>> GetReservationsByEndDate(DateTime endDate)
        {
            var result = await _dbContext.Reservations.Where(x => x.EndDate.Date == endDate.Date).ToListAsync() ?? Option<List<Reservation>>.None;

            if (result == Option<List<Reservation>>.None)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result.ValueUnsafe();
        }

        public async Task<Either<IError, List<Reservation>>> GetReservationsByProductId(Guid productId)
        {
            var result = await _dbContext.Reservations.Where(x => x.ProductId == productId).ToListAsync() ?? Option<List<Reservation>>.None;

            if (result == Option<List<Reservation>>.None)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result.ValueUnsafe();
        }

        public async Task<Either<IError, Reservation>> GetReservationByReservationId(Guid reservationId)
        {
            var result = await _dbContext.Reservations.Where(x => x.Id == reservationId).FirstOrDefaultAsync() ?? Option<Reservation>.None;

            if (result == Option<Reservation>.None)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result.ValueUnsafe();
        }

        public async Task<Either<IError, Reservation>> GetOverlappingReservation(Guid productId, DateTime startDate, DateTime endDate)
        {
            var result = await _dbContext.Reservations.Where(x => x.ProductId == productId && x.StartDate <= endDate && startDate < x.EndDate).FirstOrDefaultAsync() ?? Option<Reservation>.None;

            if (result == Option<Reservation>.None)
            {
                return AppErrors.FailedToFindReservation;
            }

            return result.ValueUnsafe();
        }

        public async Task<Either<IError, Reservation>> UpdateReservation(Reservation reservation)
        {
            var reservationToUpdate = await _dbContext.Reservations.Where(x => x.Id == reservation.Id).FirstOrDefaultAsync() ?? Option<Reservation>.None;

            if (reservationToUpdate == Option<Reservation>.None)
            {
                return AppErrors.FailedToFindReservation;
            }

            reservationToUpdate = reservation;

            if (_dbContext.SaveChangesAsync().IsCompletedSuccessfully)
            {
                return reservationToUpdate.ValueUnsafe();
            }

            return AppErrors.FailedToSaveReservation;
        }

        public async Task<Either<IError, Reservation>> CreateReservation(Reservation reservation)
        {
            await _dbContext.Reservations.AddAsync(reservation);
            if (_dbContext.SaveChangesAsync().IsCompletedSuccessfully)
            {
                return reservation;
            }

            return AppErrors.FailedToSaveReservation;
        }
    }
}
