using ACI.Reservations.Models;

namespace ACI.Reservations.Services.Interfaces
{
    public interface IReservationService
    {
        public Task<List<Reservation>> GetReservations();
    }
}
