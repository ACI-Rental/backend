using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationService.Models
{
    /// <summary>
    /// Reservation class used for EF Core to map its landscape for the database.
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// [Key]: Identification key for a image entry in the image table
        /// [Required]: cannot be null
        /// Identification key
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// [Required]: cannot be null
        /// Used to display the int value of what state the reservation is in.
        /// </summary>
        [Required]
        public int State { get; set; }
        /// <summary>
        /// [Required]: cannot be null
        /// Used to display the starting date when the reservation can be picked up
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// [Required]: cannot be null
        /// Used to display the end date when the reservation need to be brought back
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// [Required]: cannot be null
        /// Used to display the return date of the reservation
        /// </summary>
        [Required]
        public DateTime ReturnDate { get; set; }


    }
}
