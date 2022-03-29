namespace ACI.Reservations.Models.DTO
{
    public class ProductCategoryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual List<ProductDTO> Products { get; set; }
    }
}
