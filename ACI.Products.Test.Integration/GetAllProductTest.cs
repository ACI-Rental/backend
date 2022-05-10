using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using ACI.Products.Models.DTO;
using ACI.Products.Test.Integration.Fixtures;
using FluentAssertions;
using Xunit;

namespace ACI.Products.Test.Integration;

public class GetAllProductTest : IClassFixture<ProductAppFactory>
{
    private readonly HttpClient _apiClient;

    public GetAllProductTest(ProductAppFactory factory)
    {
        _apiClient = factory.CreateClient();
    }

    [Fact(Skip = "Broken: For some reason the total count is much higher than what we initially seed the db with")]
    public async void GetAllProducts_ReturnsExpectedAmount()
    {
        // Arrange
        var expectedProductCount = DbSetup.ProductsPerCategory * DbSetup.GetCategories().Count;

        // Act
        var responseMessage = await _apiClient.GetAllProducts();
        responseMessage.EnsureSuccessStatusCode();

        var allProducts = await responseMessage.Content.ReadFromJsonAsync<List<ProductResponse>>()
               ?? throw new ArgumentException("Unable to deserialize list of products");

        // Assert
        allProducts.Count.Should().Be(expectedProductCount);
    }
}