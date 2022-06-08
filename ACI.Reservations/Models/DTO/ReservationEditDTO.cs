using System;
using System.ComponentModel.DataAnnotations;

namespace ACI.Reservations.Models.DTO
{
    public class ReservationEditDTO
    {
        [Required]
        public Guid ReservationId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
