using System;
using System.ComponentModel.DataAnnotations;

namespace ACI.Reservations.Models.DTO
{
    public class ReservationActionDTO
    {
        [Required]
        public Guid ReservationId { get; set; }

        [Required]
        public int ReservationAction { get; set; }
    }
}
