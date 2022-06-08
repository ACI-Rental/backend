using System.ComponentModel.DataAnnotations;

namespace ACI.Products.Models.DTO;

public class CreateProductRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(128, ErrorMessage = "Productnaam moet tussen de 2 en 128 karakters lang zijn", MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    [MaxLength(1024)]
    public string Description { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    [MaxLength(1024)]
    public string Location { get; set; } = null!;

    [Required]
    public bool RequiresApproval { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public Product MapToModel()
    {
        return new Product
        {
            Name = Name,
            Description = Description,
            Location = Location,
            Archived = false,
            RequiresApproval = RequiresApproval,
            CategoryId = CategoryId,
        };
    }
}