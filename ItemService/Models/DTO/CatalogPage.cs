using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    /// <summary>
    /// A catalog of products containing a subset of all products.
    /// </summary>
    public class CatalogPage
    {
        /// <summary>
        /// List of all the items that are within in the page
        /// </summary>
        public List<CatalogItemsWithCategory> CatalogItems { get; set; }
        /// <summary>
        /// De current page number
        /// Sttarts with 1
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The total number of products
        /// Not the number of products in the page.
        /// </summary>
        public int TotalProductCount { get; set; }
    }
}
