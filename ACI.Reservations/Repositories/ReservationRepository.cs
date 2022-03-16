using ACI.Reservations.DBContext;
using ACI.Reservations.Models;
using ACI.Reservations.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ACI.Reservations.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ReservationDBContext _dbContext;

        public ReservationRepository(ReservationDBContext reservationDBContext)
        {
            _dbContext = reservationDBContext;
        }

        public async Task<List<Reservation>> GetReservations()
        {
            return await _dbContext.Reservations.ToListAsync();
        }
    }
}
