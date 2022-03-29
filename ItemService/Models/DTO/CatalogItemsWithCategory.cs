using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    /// <summary>
    /// Class used to combine a list of catalogitems to a category
    /// </summary>
    public class CatalogItemsWithCategory
    {
        /// <summary>
        /// List of catalogitems attached to a category
        /// </summary>
        public List<CatalogItem> CatalogItems { get; set; }
        /// <summary>
        /// Name of the category
        /// </summary>
        public string CategoryName { get; set; }
    }
}
