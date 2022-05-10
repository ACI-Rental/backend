using ACI.Products.Domain.Note;
using ACI.Products.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class NotesController : BaseController
{
    private readonly ILogger<NotesController> _logger;
    private readonly INoteService _service;

    public NotesController(ILogger<NotesController> logger, INoteService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetNotes(Guid productId)
    {
        _logger.LogInformation("Getting notes for product {ProductId}", productId);
        var result = await _service.GetNotes(productId);

        return Ok(result);
    }

    [HttpGet("{noteId:guid}")]
    public async Task<IActionResult> GetNote(Guid noteId)
    {
        _logger.LogInformation("Getting note by id {NoteId}", noteId);
        var result = await _service.GetNote(noteId);

        return result
            .Some<IActionResult>(Ok)
            .None(NotFound);
    }

    [HttpPost]
    [Authorize(Roles = "employee")]
    public async Task<IActionResult> AddNote([FromBody] CreateNoteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var user = GetUser();
        _logger.LogInformation("Adding note {CreateNoteRequest} by user {User}", request, user);

        var result = await _service.AddNote(request, user);

        return result
            .Right<IActionResult>(Ok)
            .Left(BadRequest);
    }
}