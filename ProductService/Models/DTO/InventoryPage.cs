using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    /// <summary>
    /// A page of products containing a subset of all products.
    /// </summary>
    public class InventoryPage
    {
        /// <summary>
        /// The products contained with this page of the inventory
        /// </summary>
        public IEnumerable<InventoryProduct> Products { get; set; }

        /// <summary>
        /// The current page number
        /// Starts with 0
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The total number of products
        /// Not the number of products in the page.
        /// </summary>
        public int TotalProductCount { get; set; }

    }
}
