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
        /// [Key]: Identification key for a image entry in the image table
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
    }
}
