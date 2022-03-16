using ACI.Reservations.Models;

namespace ACI.Reservations.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        public Task<List<Reservation>> GetReservations();
        public Task<List<Reservation>> GetReservationsByStartDate(DateTime startDate);
        public Task<List<Reservation>> GetReservationsByEndDate(DateTime endDate);
    }
}
