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

        public async Task<List<Reservation>> GetReservationsByStartDate(DateTime startDate)
        {
            return await _reservationRepository.GetReservationsByStartDate(startDate) ?? new List<Reservation>();
        }

        public async Task<List<Reservation>> GetReservationsByEndDate(DateTime endDate)
        {
            return await _reservationRepository.GetReservationsByEndDate(endDate) ?? new List<Reservation>();
        }
    }
}
