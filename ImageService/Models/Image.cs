using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageService.Models
{
    /// <summary>
    /// Image class used for EF Core to map its landscape for the database.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// [Key]: Identification key for an image entry in the image table
        /// [Required]: cannot be null
        /// Identification key
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// [Required]: cannot be null
        /// Used to store the byte array value of an image to store the image
        /// </summary>
        [Required]
        public byte[] Blob { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// Foreign key linked to the item in another table (table type is defined in LinkedTableType)
        /// </summary>
        [Required]
        public int LinkedKey { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// Contains enum to which table the LinkedKey (foreign key) links to
        /// </summary>
        [Required]
        public LinkedTableType LinkedTableType { get; set; }
    }
}
