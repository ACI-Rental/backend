using ACI.Images.Data.Repositories.Interfaces;
using ACI.Images.Domain.Image;
using ACI.Images.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using LanguageExt.UnitTesting;
using ACI.Images.Models;
using System.Collections.Generic;
using ACI.Images.Domain;
using Microsoft.EntityFrameworkCore;
using ACI.Images.Data;
using Microsoft.Data.Sqlite;

namespace ACI.Images.Test.Unit
{
    public class ImageServiceTests
    {
        private readonly Mock<IImageRepository> _mockImageRepo;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;

        public ImageServiceTests()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"AzureBlobStorage:UrlPrefix", "http://127.0.0.1:10000/devstoreaccount1/productimages"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            _mockImageRepo = new Mock<IImageRepository>();
            _imageService = new ImageService(_mockImageRepo.Object, Mock.Of<ILogger<ImageService>>(), _configuration);
        }

        [Fact]
        public async Task Adding_NewImage_Succeeds()
        {
            //Setup mock file using a memory stream
            var content = "This Is A Fake File";
            var fileName = "test.png";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            //create FormFile with desired data
            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

            //Arrange
            var request = new UploadImageRequest { ProductId = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D"), Image = file };

            _mockImageRepo
            .Setup(s => s.AddProductImageBlob(request.ProductId, request.Image))
            .ReturnsAsync(new ProductImageBlob { ProductId = request.ProductId, Id = new Guid("b724a2c0-5eae-4727-a5b1-d75156148c80") }); //blobId?

            //Act
            var result = await _imageService.UploadImage(request);

            //Assert
            result.ShouldBeRight(r =>
            {
                r.ProductId.Equals(request.ProductId);
                r.Id.Equals("b724a2c0-5eae-4727-a5b1-d75156148c80");
            });
        }

        [Fact]
        public async Task Adding_NewImage_Fails()
        {
            //Mock image with existing productId in db?

            //Setup mock file using a memory stream
            var content = "This Is A Fake File";
            var fileName = "test.png";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            //create FormFile with desired data
            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

            //Arrange
            var request = new UploadImageRequest { ProductId = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D"), Image = file };

            _mockImageRepo
            .Setup(s => s.AddProductImageBlob(request.ProductId, request.Image))
            .ReturnsAsync(new ProductImageBlob { ProductId = request.ProductId, Id = new Guid("b724a2c0-5eae-4727-a5b1-d75156148c80") });

            //Act
            var result = await _imageService.UploadImage(request);

            //Assert
            result.ShouldBeLeft(r =>
            {
                r.Code.Equals(AppErrors.ProductIdAlreadyExistsError);
            });
        }

        [Fact]
        public async Task Get_Image_By_ProductId_Succeeds()
        {
            //Arrange
            //Act
            //Assert
        }
    }
}