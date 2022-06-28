using System;

namespace ACI.Products.Models.DTO;

public class NoteResponse
{
    public Guid Id { get; }

    public string AuthorId { get; }

    public string AuthorName { get; }

    public string TextContent { get; }

    public DateTime CreatedUTC { get; }

    public Guid ProductId { get; }

    public NoteResponse(Guid id, string authorId, string authorName, string textContent, DateTime createdUtc, Guid productId)
    {
        Id = id;
        AuthorId = authorId;
        AuthorName = authorName;
        TextContent = textContent;
        CreatedUTC = createdUtc;
        ProductId = productId;
    }

    public static NoteResponse MapFromModel(ProductNote model) =>
        new(
            model.Id,
            model.AuthorId,
            model.AuthorName,
            model.TextContent,
            model.CreatedUTC,
            model.ProductId);
}