namespace ACI.Products.Models.DTO;

public class ProductResponse
{
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public bool RequiresApproval { get; }
    public bool IsDeleted { get; }
    public int CategoryId { get; }

    public ProductResponse(Guid id, string name, string description, bool requiresApproval, bool isDeleted, int categoryId)
    {
        Id = id;
        Name = name;
        Description = description;
        RequiresApproval = requiresApproval;
        IsDeleted = isDeleted;
        CategoryId = categoryId;
    }

    public static ProductResponse MapFromModel(Product model)
        => new(
            model.Id,
            model.Name,
            model.Description,
            model.RequiresApproval,
            model.IsDeleted,
            model.CategoryId);
}