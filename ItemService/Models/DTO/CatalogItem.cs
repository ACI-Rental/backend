using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    public class CatalogItem
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
        /// The index entry for the catalog item
        /// </summary>
        public int CatalogNumber { get; set; }
        /// <summary>
        /// Description of catalogitem
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Whether the product has to be approved before it can be rented
        /// </summary>
        public bool RequiresApproval { get; set; }
        /// <summary>
        /// The availability status of the product
        /// </summary>
        public ProductState Status { get; set; }
        /// <summary>
        /// list of images of the catalogitem
        /// </summary>
        public List<string> Images { get; set; }
        /// <summary>
        /// Image index to indicate position
        /// </summary>
        public int ImageIndex { get; set; }
        /// <summary>
        /// Start date renting the catalogitem
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// End date renting the catalogitem
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Category attached to the catalogitem
        /// </summary>
        public Category Category { get; set; }
    }
}
