using System;
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
    public async void AddNewNote_Returns_SuccessResponseWithProduct()
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
            IsDeleted = false,
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