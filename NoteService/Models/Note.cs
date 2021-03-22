using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NoteService.Models
{
    /// <summary>
    /// A note that can be either for an item or a reservation.
    /// It contains a Id, content, date when placed and a reffrence to either item or reservation.
    /// </summary>
    public class Note
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        [ForeignKey("ReservationId")]
        public virtual Reservation Reserveration { get; set; }
    }
}
