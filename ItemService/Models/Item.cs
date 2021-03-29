using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ItemService.Models
{
    /// <summary>
    /// Item class used for EF Core to map its landscape for the database.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// [Key]: Identification key for a image entry in the image table
        /// [Required]: cannot be null
        /// Identification key
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Used to sort the item according to the catelognumber entry 
        /// </summary>
        public int CatelogNumber { get; set; }
        /// <summary>
        /// Displays the item's name
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Displays the item's description
        /// </summary>
        [Required]
        public string Description { get; set;}
        /// <summary>
        /// Displays the item's inventory location
        /// </summary>
        public string InventoryLocation { get; set; }
        /// <summary>
        /// Displays if the item is available to borrow
        /// </summary>
        [Required]
        public bool IsAvailable { get; set; }
        /// <summary>
        /// Used to check when the item was archived
        /// </summary>
        public DateTime ArchivedSince { get; set; }

        /// <summary>
        /// Foreign key to the category that is bound to the item
        /// </summary>
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
