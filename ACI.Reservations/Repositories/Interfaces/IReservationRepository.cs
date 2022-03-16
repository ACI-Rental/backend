using ACI.Reservations.Models;

namespace ACI.Reservations.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        public Task<List<Reservation>> GetReservations();
    }
}
