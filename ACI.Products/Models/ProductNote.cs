using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACI.Products.Models;

public class ProductNote
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string AuthorId { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string AuthorName { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    [MaxLength(1024)]
    public string TextContent { get; set; } = null!;

    [Required]
    public DateTime CreatedUTC { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public virtual Product Product { get; set; } = null!;
}