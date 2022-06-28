using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACI.Products.Models;
using LanguageExt;

namespace ACI.Products.Data.Repositories.Interfaces;

public interface INoteRepository
{
    public Task<List<ProductNote>> GetNotes(Guid productId);

    public Task<Option<ProductNote>> GetNote(Guid noteId);

    public Task<ProductNote> AddNote(ProductNote note);
}