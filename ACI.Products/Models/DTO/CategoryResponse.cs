namespace ACI.Products.Models.DTO;

public class CategoryResponse
{
    public int Id { get; }
    public string Name { get; }

    public CategoryResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static CategoryResponse MapFromModel(ProductCategory model) => new(model.Id, model.Name);
}