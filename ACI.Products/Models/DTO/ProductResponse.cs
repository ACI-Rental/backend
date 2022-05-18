using System;

namespace ACI.Products.Models.DTO;

public class ProductResponse
{
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string Location { get; }
    public bool RequiresApproval { get; }
    public bool IsDeleted { get; }
    public int CategoryId { get; }
    public string CategoryName { get; }
    public int CatalogPosition { get; }

    public ProductResponse(Guid id, string name, string description, string location, bool requiresApproval, bool isDeleted, int categoryId, string categoryName, int catalogPosition)
    {
        Id = id;
        Name = name;
        Description = description;
        Location = location;
        RequiresApproval = requiresApproval;
        IsDeleted = isDeleted;
        CategoryId = categoryId;
        CategoryName = categoryName;
        CatalogPosition = catalogPosition;
    }

    public static ProductResponse MapFromModel(Product model)
        => new(
            model.Id,
            model.Name,
            model.Description,
            model.Location,
            model.RequiresApproval,
            model.IsDeleted,
            model.CategoryId,
            model.Category?.Name,
            model.CatalogPosition);
}