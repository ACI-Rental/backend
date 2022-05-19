using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACI.Products.Models.DTO;
using LanguageExt;

namespace ACI.Products.Domain.Note;

public interface INoteService
{
    public Task<List<NoteResponse>> GetNotes(Guid productId);

    public Task<Option<NoteResponse>> GetNote(Guid noteId);

    public Task<Either<IError, NoteResponse>> AddNote(CreateNoteRequest request, AppUser author);
}