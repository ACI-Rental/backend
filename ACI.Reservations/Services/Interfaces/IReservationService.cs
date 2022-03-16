using ACI.Reservations.Models;

namespace ACI.Reservations.Services.Interfaces
{
    public interface IReservationService
    {
        public Task<List<Reservation>> GetReservations();
        public Task<List<Reservation>> GetReservationsByStartDate(DateTime startDate);
        public Task<List<Reservation>> GetReservationsByEndDate(DateTime endDate);
    }
}
