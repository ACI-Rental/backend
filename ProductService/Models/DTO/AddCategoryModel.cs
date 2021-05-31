using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    /// <summary>
    /// Data model for the AddCategoryModel calls
    /// </summary>
    public class AddCategoryModel
    {
        /// <summary>
        /// Used to store the name of the new category
        /// </summary>
        public string Name { get; set; }
    }
}
