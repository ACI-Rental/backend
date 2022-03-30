using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationService.Models.DTO
{
    /// <summary>
    /// A page of reservations containing a subset of all reservations
    /// </summary>
    public class ReservationOverviewPage
    {
        /// <summary>
        /// The Reservations contained with this page
        /// </summary>
        public List<List<Reservation>> Reservations { get; set; }

        /// <summary>
        /// The current page number
        /// Starts with 0
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The total number of reservations
        /// Not the number of reservations on the page
        /// </summary>
        public int TotalReservationCount { get; set; }
    }
}
