using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using LanguageExt;

namespace ACI.Reservations.Services.Interfaces
{
    public interface IReservationService
    {
        public Task<Either<IError, List<Reservation>>> GetReservations();
        public Task<Either<IError, List<Reservation>>> GetReservationsByStartDate(DateTime startDate);
        public Task<Either<IError, List<Reservation>>> GetReservationsByEndDate(DateTime endDate);
        public Task<Either<IError, List<Reservation>>> GetReservationsByProductId(Guid productId);
        public Task<Either<IError, Reservation>> ExecuteReservationAction(Guid reservationId, ReservationAction action);
        public Task<Either<IError, Reservation>> ReserveProduct(ProductReservationDTO productReservationDTO);
    }
}
