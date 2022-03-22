using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using LanguageExt;

namespace ACI.Reservations.Services.Interfaces
{
    public interface IReservationService
    {
        public Task<List<Reservation>> GetReservations();
        public Task<List<Reservation>> GetReservationsByStartDate(DateTime startDate);
        public Task<List<Reservation>> GetReservationsByEndDate(DateTime endDate);
        public Task<List<Reservation>> GetReservationsByProductId(Guid productId);
        public Task<Either<IError, Reservation>> ExecuteReservationAction(Guid reservationId, ReservationAction action);
        public Task<Either<IError, Reservation>> ReserveProduct(ProductReservationDTO productReservationDTO);
    }
}
