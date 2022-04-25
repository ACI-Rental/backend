using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ACI.Products.Domain;
using ACI.Products.Models.DTO;
using ACI.Products.Test.Integration.Fixtures;
using FluentAssertions;
using Xunit;

namespace ACI.Products.Test.Integration;

public class ProductTest : IClassFixture<ProductAppFactory>
{
    private readonly HttpClient _apiClient;

    public ProductTest(ProductAppFactory factory)
    {
        _apiClient = factory.CreateClient();
    }

    [Fact]
    public async void AddNewCategory_Returns_SuccessResponseWithCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Keyboards" };

        // Act
        var response = await _apiClient.PostCreateCategory(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        category.Should().NotBeNull();
        category!.Id.Should().BePositive();
        category.Name.Should().Be(request.Name);
    }

    [Fact]
    public async void AddDuplicateCategory_Returns_ErrorResponse()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "TestDuplicateCategory" };

        // Act
        var response = await _apiClient.PostCreateCategory(request);
        var secondResponse = await _apiClient.PostCreateCategory(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var expectedError = AppErrors.CategoryNameAlreadyExistsError;
        var error = await secondResponse.Content.ReadFromJsonAsync<IError>();

        error.Should().NotBeNull();
        error.Should().Be(expectedError);
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
        var response = await _apiClient.PostCreateProduct(request);

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

    [Fact]
    public async void GetProductById_Returns_SuccessResult()
    {
        // Arrange
        var allProducts = await GetAllProducts();
        var searchProduct = allProducts.First();

        // Act
        var result = await _apiClient.GetProductById(searchProduct.Id);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var foundProduct = await result.Content.ReadFromJsonAsync<ProductResponse>();

        foundProduct.Should().BeEquivalentTo(searchProduct);
    }

    [Fact]
    public async void DeleteProductById_Returns_Success()
    {
        // Arrange
        var res = await _apiClient.GetAllProducts();
        res.EnsureSuccessStatusCode();

        var allProducts = await res.Content.ReadFromJsonAsync<List<ProductResponse>>()
               ?? throw new ArgumentException("Unable to deserialize list of products");

        var searchProduct = allProducts.First();

        // Act
        var delResult = await _apiClient.DeleteProductById(searchProduct.Id);
        var getResult = await _apiClient.GetProductById(searchProduct.Id);

        // Assert
        delResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<List<ProductResponse>> GetAllProducts()
    {
        var result = await _apiClient.GetAllProducts();
        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<List<ProductResponse>>()
               ?? throw new ArgumentException("Unable to deserialize list of products");
    }
}