using System.ComponentModel.DataAnnotations;

namespace ACI.Products.Models.DTO;

public class CreateCategoryRequest
{
    [Required]
    [StringLength(32, ErrorMessage = "Categorie naam moet tussen de 2 en 32 karakters lang zijn!", MinimumLength = 2)]
    public string Name { get; set; } = null!;
}