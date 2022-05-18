using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ACI.Products.Models.DTO;
using ACI.Products.Test.Integration.Fixtures;
using FluentAssertions;
using Xunit;

namespace ACI.Products.Test.Integration;

public class NoteTest : IClassFixture<ProductAppFactory>
{
    private readonly HttpClient _apiClient;

    public NoteTest(ProductAppFactory factory)
    {
        _apiClient = factory.CreateClient();
    }

    [Fact]
    public async void AddNewNote_Returns_SuccessResponse()
    {
        // Arrange
        var product = await InsertProduct("Canon ES133", "30MP Camera");

        var request = new CreateNoteRequest(product.Id, "Scratches on the display");

        // Act
        var response = await _apiClient.PostCreateNote(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var note = await response.Content.ReadFromJsonAsync<NoteResponse>();

        note.Should().NotBeNull();
        note.Id.Should().NotBeEmpty();
        note.ProductId.Should().Be(request.ProductId);
        note.AuthorId.Should().Be(ProductAppFactory.DefaultUserId);
        note.TextContent.Should().Be(request.TextContent);
        note.AuthorName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async void GetAllNotes_Returns_SuccessResult()
    {
        // Arrange
        var product = await InsertProduct("JBL Speaker", "USB-C port, 3.5mm jack");

        var textContent = new List<string> { "Missing case", "Scratches on bottom", "Broken volume dial" };

        var createNoteRequests = textContent.Select(text => new CreateNoteRequest(product.Id, text)).ToList();

        // Act
        await InsertNotes(createNoteRequests);

        // Assert
        var result = await _apiClient.GetAllNotes(product.Id);
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var notes = await result.Content.ReadFromJsonAsync<List<NoteResponse>>();

        notes.Should().NotBeNullOrEmpty();
        notes.Count.Should().Be(textContent.Count);

        // Making sure every text content turned into a note (no duplicates)
        foreach (var text in textContent)
        {
            notes.Should().ContainSingle(n => n.TextContent.Equals(text));
        }

        foreach (var note in notes)
        {
            note.Should().NotBeNull();
            note.Id.Should().NotBeEmpty();
            note.AuthorId.Should().Be(ProductAppFactory.DefaultUserId);
            note.ProductId.Should().Be(product.Id);
            note.CreatedUTC.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }
    }

    [Fact]
    public async void GetSingleNote_Returns_SuccessResult()
    {
        // Arrange
        var product = await InsertProduct("Bose SoundSystem", "Bluetooth, Airplay");
        var payload = new List<CreateNoteRequest> { new(product.Id, "Lorem ipsum") };
        var addedNote = (await InsertNotes(payload)).First();

        // Act
        var result = await _apiClient.GetNoteById(addedNote.Id);

        // Assert
        var note = await result.Content.ReadFromJsonAsync<NoteResponse>();

        note.Should().NotBeNull();
        note.Id.Should().Be(addedNote.Id);
        note.AuthorId.Should().Be(ProductAppFactory.DefaultUserId);
        note.TextContent.Should().Be(addedNote.TextContent);
    }

    private async Task<List<NoteResponse>> InsertNotes(List<CreateNoteRequest> noteRequests)
    {
        var responses = new List<NoteResponse>();

        foreach (var req in noteRequests)
        {
            var res = await _apiClient.PostCreateNote(req);
            res.EnsureSuccessStatusCode();

            var note = await res.Content.ReadFromJsonAsync<NoteResponse>();
            responses.Add(note ?? throw new ApplicationException("Unable to deserialize NoteResponse"));
        }

        return responses;
    }

    private async Task<ProductResponse> InsertProduct(
        string name = "TestProduct",
        string description = "TestDescription",
        int category = 1)
    {
        var request = new CreateProductRequest
        {
            CategoryId = category,
            Description = description,
            Name = name,
            RequiresApproval = false,
        };

        return await InsertProduct(request);
    }

    private async Task<ProductResponse> InsertProduct(CreateProductRequest req)
    {
        var response = await _apiClient.PostCreateProduct(req);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return await response.Content.ReadFromJsonAsync<ProductResponse>() ??
               throw new ApplicationException("Unable to insert product");
    }
}