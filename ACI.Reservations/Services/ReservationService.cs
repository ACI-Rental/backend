using ACI.Reservations.DBContext;
using ACI.Reservations.Models;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Reservations.Services.Interfaces;

namespace ACI.Reservations.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<List<Reservation>> GetReservations()
        {
            return await _reservationRepository.GetReservations();
        }
    }
}
