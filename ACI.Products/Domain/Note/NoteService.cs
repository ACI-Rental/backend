using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models;
using ACI.Products.Models.DTO;
using LanguageExt;

namespace ACI.Products.Domain.Note;

public class NoteService : INoteService
{
    private readonly IProductRepository _productRepository;
    private readonly INoteRepository _noteRepository;

    public NoteService(INoteRepository noteRepository, IProductRepository productRepository)
    {
        _noteRepository = noteRepository;
        _productRepository = productRepository;
    }

    public async Task<List<NoteResponse>> GetNotes(Guid productId)
    {
        return (await _noteRepository.GetNotes(productId))
            .Select(NoteResponse.MapFromModel)
            .ToList();
    }

    public async Task<Option<NoteResponse>> GetNote(Guid noteId)
    {
        var result = await _noteRepository.GetNote(noteId);

        return result.Map(NoteResponse.MapFromModel);
    }

    public async Task<Either<IError, NoteResponse>> AddNote(CreateNoteRequest request, AppUser author)
    {
        var product = await _productRepository.GetProductById(request.ProductId);

        if (product.IsNone)
        {
            return AppErrors.ProductNotFoundError;
        }

        var note = new ProductNote
        {
            AuthorId = author.Id,
            AuthorName = author.Name,
            CreatedUTC = DateTime.UtcNow,
            ProductId = request.ProductId,
            TextContent = request.TextContent,
        };

        var result = await _noteRepository.AddNote(note);
        return NoteResponse.MapFromModel(result);
    }
}