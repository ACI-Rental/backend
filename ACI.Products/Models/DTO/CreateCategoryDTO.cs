namespace ACI.Products.Models.DTO;

public class CreateCategoryDTO
{
    public string Name { get; set; }

    public ProductCategory ToCategory()
    {
        return new ProductCategory { Name = Name };
    }
}