using System;

namespace ACI.Reservations.Models.DTO
{
    public class ReservationCreateDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ProductId { get; set; }
    }
}
