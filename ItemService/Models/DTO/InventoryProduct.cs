using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    /// <summary>
    /// The product data that gets displayed in the inventory
    /// </summary>
    public class InventoryProduct
    {
        /// <summary>
        /// The id of the product
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the product
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The location where the product is physically stored
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Whether the product has to be approved before it can be rented
        /// </summary>
        public bool RequiresApproval { get; set; }
        /// <summary>
        /// The availability status of the product
        /// </summary>
        public InventoryProductStatus Status { get; set; }
    }
}
