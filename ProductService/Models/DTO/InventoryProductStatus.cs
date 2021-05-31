using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    /// <summary>
    /// The statusses a product can have
    /// </summary>
    public enum InventoryProductStatus
    {
        Available = 1,
        Unavailable = 2
    }
}
