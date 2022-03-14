using System.ComponentModel.DataAnnotations;

namespace ACI.Products.Models.DTO;

public class CreateProductDTO
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(128)]
    public string Name { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    [MaxLength(1024)]
    public string Description { get; set; } = null!;

    [Required]
    public bool IsDeleted { get; set; }

    [Required]
    public bool RequiresApproval { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public Product ToProduct()
    {
        return new Product
        {
            Name = Name,
            Description = Description,
            IsDeleted = IsDeleted,
            RequiresApproval = RequiresApproval,
            CategoryId = CategoryId,
        };
    }
}