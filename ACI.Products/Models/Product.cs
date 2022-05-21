using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACI.Products.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

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
    public DateTime CreatedUTC { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public virtual ProductCategory Category { get; set; } = null!;

}