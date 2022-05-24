using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACI.Reservations.Models
{
    public class Reservation
    {
        /// <summary>
        /// Gets or sets [Key]: Identification key for a image entry in the image table
        /// [Required]: cannot be null
        /// Identification key.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets [Required]: cannot be null
        /// Used to display the starting date when the reservation can be picked up.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets [Required]: cannot be null
        /// Used to display the end date when the reservation need to be brought back.
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets used to display the pickup date of the reservation.
        /// </summary>
        public DateTime? PickedUpDate { get; set; }

        /// <summary>
        /// Gets or sets used to display the return date of the reservation.
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets [Required]: cannot be null
        /// Id of the person that mode the reservation.
        /// </summary>
        [Required]
        public string RenterId { get; set; }

        /// <summary>
        /// Gets or sets id of the person that made the review of the reservation.
        /// </summary>
        public string? ReviewerId { get; set; }

        /// <summary>
        /// Gets or sets used to know if the reservation is approved or not.
        /// </summary>
        public bool? IsApproved { get; set; }

        /// <summary>
        /// Gets or sets [Required]: cannot be null
        /// Used to represent the product that is being reserved.
        /// </summary>
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether used to know if a reservation is cancelled.
        /// </summary>
        public bool Cancelled { get; set; } = false;
    }
}
