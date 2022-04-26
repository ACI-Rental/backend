using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using ACI.Images.Domain;
using ACI.Images.Models.DTO;
using ACI.Images.Test.Integration.Fixtures;
using FluentAssertions;
using Xunit;

namespace Aci.Images.Test.Integration
{
    public class ImageTest : IClassFixture<ImageAppFactory>
    {
        private readonly HttpClient _apiClient;

        public ImageTest(ImageAppFactory factory)
        {
            _apiClient = factory.CreateClient();
        }

        [Fact]
        public async void AddNewImage_Returns_SuccessResponse()
        {
            await using var stream = System.IO.File.OpenRead("./TestPhoto/camera.jpg");

            var payload = new
            {
                ProductId = "62FA647C-AD54-4BCC-A860-E5A2664B019F"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "image");

            using var content = new MultipartFormDataContent
            {
                // file
                { new StreamContent(stream), "Image", "camera.jpg" },

                // payload
                { new StringContent(payload.ProductId), "ProductId" },
            };

            request.Content = content;

            var result = await _apiClient.SendAsync(request);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void AddNewImage_Returns_ErrorResponse()
        {
            await using var stream = System.IO.File.OpenRead("./TestPhoto/camera.jpg");

            var payload = new
            {
                //image with this productId should already exist
                ProductId = "62FA647C-AD54-4BCC-A860-E5A2664B019D" 
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "image");

            using var content = new MultipartFormDataContent
            {
                // file
                { new StreamContent(stream), "Image", "camera.jpg" },

                // payload
                { new StringContent(payload.ProductId), "ProductId" },
            };

            request.Content = content;

            var result = await _apiClient.SendAsync(request);

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var expectedError = AppErrors.ProductIdAlreadyExistsError;
            var error = await result.Content.ReadFromJsonAsync<IError>();

            error.Should().NotBeNull();
            error.Should().Be(expectedError);
        }

        [Fact]
        public async void GetImageByProductId_Returns_SuccessResult()
        {
            // Arrange
            var productId = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D");

            // Act
            var result = await _apiClient.GetImageByProductId(productId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var foundImage = await result.Content.ReadFromJsonAsync<ImageResponse>();

            foundImage.ProductId.Should().Be(productId);
        }

        [Fact]
        public async void GetImageByProductId_Returns_ErrorResult()
        {
            // Arrange
            var productId = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019E"); //Should be no image with this guid

            // Act
            var result = await _apiClient.GetImageByProductId(productId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var expectedError = AppErrors.ImageNotFoundError;
            var error = await result.Content.ReadFromJsonAsync<IError>();

            error.Should().NotBeNull();
            error.Should().Be(expectedError);
        }

        [Fact]
        public async void DeleteImageByProductId_Returns_ErrorResult()
        {
            // Arrange
            var productId = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019E"); //No image with this productId

            // Act
            var result = await _apiClient.DeleteImageByProductId(productId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var expectedError = AppErrors.ImageAlreadyDeletedError;
            var error = await result.Content.ReadFromJsonAsync<IError>();

            error.Should().NotBeNull();
            error.Should().Be(expectedError);
        }
    }
}