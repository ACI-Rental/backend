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
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public int ProductId { get; set; }
       
        public int ReservationId { get; set; }
    }
}
