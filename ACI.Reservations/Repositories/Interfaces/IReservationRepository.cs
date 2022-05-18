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
        public Task<Either<IError, List<ReservationDTO>>> GetReservations();
        public Task<Either<IError, List<ReservationDTO>>> GetReservationsByStartDate(DateTime startDate);
        public Task<Either<IError, List<ReservationDTO>>> GetReservationsByEndDate(DateTime endDate);
        public Task<Either<IError, List<ReservationDTO>>> GetReservationsByProductId(Guid productId);
        public Task<Either<IError, ReservationDTO>> GetReservationByReservationId(Guid reservationId);
        public Task<Either<IError, ReservationDTO>> GetOverlappingReservation(Guid productId, DateTime startDate, DateTime endDate);
        public Task<Either<IError, ReservationDTO>> UpdateReservation(ReservationDTO reservation);
        public Task<Either<IError, ReservationDTO>> CreateReservation(Reservation reservation);
    }
}
