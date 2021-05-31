using System;

namespace ProductService.Models.DTO
{
    /// <summary>
    /// Stripped and basic product model based on the product.cs database model
    /// Used for calls that do not require all data and just one image
    /// </summary>
    public class ProductFlatModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public ProductState ProductState { get; set; }
    }
}
