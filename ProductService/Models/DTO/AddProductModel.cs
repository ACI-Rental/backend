namespace ProductService.Models.DTO
{
    /// <summary>
    /// Data model for the AddProduct calls
    /// </summary>
    public class AddProductModel
    {
        /// <summary>
        /// Used to sort the products according to the catelognumber entry 
        /// </summary>
        public int CatalogNumber { get; set; }
        /// <summary>
        /// Displays the product name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Displays the product description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Displays the product inventory location
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Boolean if product rental needs to be approved
        /// </summary>
        public bool RequiresApproval { get; set; }
        /// <summary>
        /// ForeignKey to the Category table
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Images that have to be linked to the product
        /// </summary>
        public string[] Images { get; set; }
    }
}
