using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationService.Models.DTO
{
    /// <summary>
    /// Data model for the reserve product calls
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// Identification key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The localId for the front-end reload
        /// </summary>
        public int LocalId { get; set; }
        /// <summary>
        /// Start date for the reservation of a product
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// End date for the reservation of a product
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
