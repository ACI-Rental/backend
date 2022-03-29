using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models
{
    /// <summary>
    /// Category class used for EF Core to map its landscape for the database.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// [Key]: Identification key for a category entry in the category table
        /// [Required]: cannot be null
        /// Identification key
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// [Required]: cannot be null
        /// Displays the category's name
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
