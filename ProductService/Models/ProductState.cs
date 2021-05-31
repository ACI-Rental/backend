using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    /// <summary>
    /// Enum for the state of a specific product
    /// </summary>
    public enum ProductState
    {
        AVAILABLE = 0,
        UNAVAILABLE = 1,
        ARCHIVED = 2
    }
}
