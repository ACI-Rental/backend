using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationService.Models.DTO
{
    /// <summary>
    /// Data model for the reserve product calls
    /// </summary>
    public class ReserveProductModel
    {
        /// <summary>
        /// List of products models that the user wants to reserve
        /// </summary>
        public List<ProductModel> ProductModels { get; set; }
    }
}
