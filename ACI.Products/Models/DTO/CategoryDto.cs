namespace ACI.Products.Models.DTO;

public class CategoryDto
{
    public int Id { get; }
    public string Name { get; }

    public CategoryDto(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static CategoryDto From(ProductCategory model) => new(model.Id, model.Name);
}