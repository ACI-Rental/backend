using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using LanguageExt;

namespace ACI.Reservations.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        public Task<Either<IError, List<Reservation>>> GetReservations();
        public Task<Either<IError, List<Reservation>>> GetReservationsByStartDate(DateTime startDate);
        public Task<Either<IError, List<Reservation>>> GetReservationsByEndDate(DateTime endDate);
        public Task<Either<IError, List<Reservation>>> GetReservationsByProductId(Guid productId);
        public Task<Either<IError, Reservation>> GetReservationByReservationId(Guid reservationId);
        public Task<Either<IError, Reservation>> GetOverlappingReservation(Guid productId, DateTime startDate, DateTime endDate);
        public Task<Either<IError, Reservation>> UpdateReservation(Reservation reservation);
        public Task<Either<IError, Reservation>> CreateReservation(Reservation reservation);
    }
}
