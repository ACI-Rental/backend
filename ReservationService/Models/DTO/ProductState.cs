using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationService.Models.DTO
{
    /// <summary>
    /// enum to set the state a product is in
    /// </summary>
    public enum ProductState
    {
        AVAILABLE = 0,
        UNAVAILABLE = 1,
        ARCHIVED = 2
    }
}
