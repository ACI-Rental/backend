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
        public Task<Either<IError, List<ReservationDTO>>> GetReservations();
        public Task<Either<IError, List<ReservationDTO>>> GetReservationsByStartDate(DateTime startDate);
        public Task<Either<IError, List<ReservationDTO>>> GetReservationsByEndDate(DateTime endDate);
        public Task<Either<IError, List<ReservationDTO>>> GetReservationsByProductId(Guid productId);
        public Task<Either<IError, ReservationDTO>> ExecuteReservationAction(Guid reservationId, ReservationAction action);
        public Task<Either<IError, ReservationDTO>> ReserveProduct(ReservationDTO ReservationDTO);
    }
}
