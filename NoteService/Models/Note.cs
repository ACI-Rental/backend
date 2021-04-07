using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NoteService.Models
{
    /// <summary>
    /// A note that can be either for a product or a reservation.
    /// It contains a Id, content, date when placed and a reffrence to either product or reservation.
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
        /// Refrence to the Item, the note is attached to
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// [Column] gives the colomn a specific name
        /// Refrence to the Reservation, the note is attached to
        /// </summary>
        public int ReserverationId { get; set; }
    }
}
