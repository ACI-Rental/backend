using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace ACI.Products.Data.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly ProductContext _ctx;

    public NoteRepository(ProductContext ctx, ILogger<NoteRepository> logger)
    {
        _ctx = ctx;
    }

    public async Task<List<ProductNote>> GetNotes(Guid productId)
    {
        return await _ctx.Notes
            .Include(x => x.Product)
            .ThenInclude(x => x.Category)
            .Where(n => n.ProductId == productId)
            .ToListAsync();
    }

    public async Task<Option<ProductNote>> GetNote(Guid noteId)
    {
        var note = await _ctx.Notes
            .Include(x => x.Product)
            .ThenInclude(x => x.Category)
            .FirstOrDefaultAsync(n => n.Id == noteId);

        return note ?? Option<ProductNote>.None;
    }

    public async Task<ProductNote> AddNote(ProductNote note)
    {
        var result = await _ctx.Notes.AddAsync(note);
        await _ctx.SaveChangesAsync();

        return result.Entity;
    }
}