using ACI.Images.Models.DTO;
using ACI.Images.Test.Integration.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
            //Arrange
            var expectedContentType = "text/html; charset=utf-8";

            // Act
            var file = File.OpenRead(@"TestPhoto\camera.jpg");
            HttpContent fileStreamContent = new StreamContent(file);

            var formData = new MultipartFormDataContent
            {
                { fileStreamContent, "camera", "camera.jpg" }
            };

            var request = new UploadImageRequest { ProductId = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D"), Image = formData };

            var response = await _apiClient.PostCreateImage(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.NotEmpty(responseString);
            Assert.Equal(expectedContentType, response.Content.Headers.ContentType.ToString());

            response.Dispose();
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

            foundImage.ProductId.Equals(productId);
        }
    }
}