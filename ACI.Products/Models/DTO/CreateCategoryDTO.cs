using System.ComponentModel.DataAnnotations;

namespace ACI.Products.Models.DTO;

public class CreateCategoryDTO
{
    [Required]
    [Range(2, 32, ErrorMessage = "Categorie-naam moet tussen de 2 en 32 karakters lang zijn!")]
    public string Name { get; set; } = null!;

    public ProductCategory ToCategory()
    {
        return new ProductCategory { Name = Name };
    }
}