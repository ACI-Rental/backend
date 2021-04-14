using ProductService.Models.DTO;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models
{
    /// <summary>
    /// Product class used for EF Core to map its landscape for the database.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// [Key]: Identification key for a product entry in the product table
        /// [Required]: cannot be null
        /// Identification key
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Used to sort the product according to the catelognumber entry 
        /// </summary>
        public int CatalogNumber { get; set; }
        /// <summary>
        /// Displays the product name
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Displays the product description
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Displays the product inventory location
        /// </summary>
        public string InventoryLocation { get; set; }
        /// <summary>
        /// Sets one of the following states for the product: available, unavailable or archived
        /// </summary>
        [Required]
        public ProductState ProductState { get; set; }
        /// <summary>
        /// Boolean if product rental needs to be approved
        /// </summary>
        [Required]
        public bool RequiresApproval { get; set; }
        /// <summary>
        /// Foreign key to the category that is bound to the product
        /// [Required]: cannot be null
        /// </summary>
        [ForeignKey("CategoryId")]
        [Required]
        public virtual Category Category { get; set; }
    }
}
