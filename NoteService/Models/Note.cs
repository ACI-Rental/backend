using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NoteService.Models
{
    /// <summary>
    /// <b>Note</b> class used for EF Core to map its landscape for the database.
    /// It contains a Id, content, date when placed and a reffrence to either item or reservation.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// [Key]: Identification key for a Note entry in the Note table
        /// [Required]: cannot be null
        /// Identification key
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// Textual content of the note
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// Date of when the note was placed
        /// </summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// [Column] gives the colomn a specific name
        /// Refrence to the Item, the note is attached to
        /// </summary>
        [Column("ItemId")]
        public int Item { get; set; }

        /// <summary>
        /// [Column] gives the colomn a specific name
        /// Refrence to the Reservation, the note is attached to
        /// </summary>
        [Column("ReservationId")]
        public int Reserveration { get; set; }
    }
}
