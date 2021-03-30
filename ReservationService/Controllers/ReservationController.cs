using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationService.DBContexts;
using ReservationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationService.Controllers
{
    /// <summary>
    /// Reservation controller this controller is used for the calls between API and frontend for managing the reservations in the ACI Rental system
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ReservationController : ControllerBase
    {
        /// <summary>
        /// Database context for the reservation service, this is used to make calls to the reservation table
        /// </summary>
        private readonly ReservationServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructor is used for receiving the database context at the creation of the image controller
        /// </summary>
        /// <param name="dbContext">Database context param used for calls to the reservation table</param>
        public ReservationController(ReservationServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _dbContext.Reservations.ToListAsync();
        }
    }
}
