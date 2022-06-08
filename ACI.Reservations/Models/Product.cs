using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACI.Reservations.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(128)]
        public string Name { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        [MaxLength(1024)]
        public string Location { get; set; }

        [Required]
        public bool RequiresApproval { get; set; }

        [Required]
        public bool Archived { get; set; }

        [Required]
        public string CategoryName { get; set; }

        [Required]
        public virtual List<Reservation> Reservations { get; set; }
    }
}