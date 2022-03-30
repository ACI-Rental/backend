using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACI.Reservations.Models
{
    public class Reservation
    {
        /// <summary>
        /// [Key]: Identification key for a image entry in the image table
        /// [Required]: cannot be null
        /// Identification key.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// Used to display the starting date when the reservation can be picked up.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// Used to display the end date when the reservation need to be brought back.
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Used to display the pickup date of the reservation.
        /// </summary>
        public DateTime? PickedUpDate { get; set; }

        /// <summary>
        /// Used to display the return date of the reservation.
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// Id of the person that mode the reservation.
        /// </summary>
        [Required]
        public Guid RenterId { get; set; }

        /// <summary>
        /// Id of the person that made the review of the reservation.
        /// </summary>
        public Guid? ReviewerId { get; set; }

        /// <summary>
        /// Used to know if the reservation is approved or not.
        /// </summary>
        public bool? IsApproved { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// Used to represent the product that is being reserved.
        /// </summary>
        [Required]
        public Guid ProductId { get; set; }

        /// <summary>
        /// Used to know if a reservation is cancelled.
        /// </summary>
        public bool Cancelled { get; set; } = false;
    }
}
