using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationService.Models.DTO
{
    /// <summary>
    /// Product class based on the product.cs of the ProductService
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Identification key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Sets one of the following states for the product: available, unavailable or archived
        /// </summary>
        public ProductState ProductState { get; set; }
        /// <summary>
        /// Boolean if product rental needs to be approved
        /// </summary>
        public bool RequiresApproval { get; set; }
    }
}
