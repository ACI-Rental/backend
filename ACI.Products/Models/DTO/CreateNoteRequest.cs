using System;
using System.ComponentModel.DataAnnotations;

namespace ACI.Products.Models.DTO;

public class CreateNoteRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(1024)]
    public string TextContent { get; }

    [Required]
    public Guid ProductId { get; }

    public CreateNoteRequest(Guid productId, string textContent)
    {
        TextContent = textContent;
        ProductId = productId;
    }
}