using ProductService.Models;
using ProductService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Converters
{
    /// <summary>
    /// Converter used for the product controller to convert to catalog items
    /// </summary>
    public class CatalogItemConverter
    {
        /// <summary>
        /// Converts a regular product class to a catalogitem
        /// </summary>
        /// <param name="product"></param>
        /// <param name="catalogImages"></param>
        /// <returns>Returns a converted product model to CatalogItem model</returns>
        public CatalogItem ConvertProductToCatalogItemAsync(Product product, List<string> catalogImages)
        {
            CatalogItem item = new()
            {
                Id = product.Id,
                Category = product.Category,
                CatalogNumber = product.CatalogNumber,
                Description = product.Description,
                Name = product.Name,
                RequiresApproval = product.RequiresApproval,
                Status = product.ProductState,
                Images = catalogImages,
                ImageIndex = 0,
            };
            return item;
        }

        /// <summary>
        /// Adds new entry to a cataloglist
        /// </summary>
        /// <param name="item"></param>
        /// <param name="catalogItem"></param>
        /// <returns>Returns a list of catalogitems with their category</returns>
        public CatalogItemsWithCategory AddNewEntryToCatalogList(Product item, List<string> catalogItem)
        {
            CatalogItemsWithCategory catalogList = new()
            {
                CatalogItems = new List<CatalogItem>()
                {
                    ConvertProductToCatalogItemAsync(item, catalogItem)
                },
                CategoryName = item.Category.Name
            };
            return catalogList;
        }
    }
}
