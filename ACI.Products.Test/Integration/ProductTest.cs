using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using ACI.Products.Models.DTO;
using FluentAssertions;
using Xunit;

namespace ACI.Products.Test.Integration;

public class ProductTest : IClassFixture<ProductServiceApplication>
{
    private readonly HttpClient _client;

    public ProductTest(ProductServiceApplication factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async void AddNewCategory_Returns_SuccessResponseWithCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Keyboards" };

        // Act
        var response = await _client.PostAsJsonAsync("category", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        category.Should().NotBeNull();
        category!.Id.Should().BePositive();
        category.Name.Should().Be(request.Name);
    }

    [Fact]
    public async void AddNewProduct_Returns_SuccessResponseWithProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            CategoryId = 6,
            Description = "Some Description",
            Name = "Macbook Pro",
            IsDeleted = false,
            RequiresApproval = false,
        };

        // Act
        var response = await _client.PostAsJsonAsync("products", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();

        product.Should().NotBeNull();
        product!.Id.Should().NotBeEmpty();
        product.RequiresApproval.Should().Be(request.RequiresApproval);
        product.IsDeleted.Should().BeFalse();
        product.Name.Should().Be(request.Name);
        product.Description.Should().Be(request.Description);
        product.CategoryId.Should().Be(request.CategoryId);
    }
}