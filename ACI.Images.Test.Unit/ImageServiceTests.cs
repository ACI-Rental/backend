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
using LanguageExt;

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
            .ReturnsAsync(new ProductImageBlob { ProductId = request.ProductId, Id = new Guid("b724a2c0-5eae-4727-a5b1-d75156148c80") });

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
        public async Task Adding_NewImage_Fails_ExistingProductId()
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
            .ReturnsAsync(AppErrors.ProductIdAlreadyExistsError);

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
            var request = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D");
            var blobId = "testString";

            _mockImageRepo
            .Setup(s => s.GetProductImageBlobByProductId(request))
            .ReturnsAsync(new ProductImageBlob { ProductId = new Guid("022e4d43-e585-4eb2-b864-0faab2bf3a4d"), Id = new Guid("b724a2c0-5eae-4727-a5b1-d75156148c80"), BlobId = blobId });

            _mockImageRepo
            .Setup(s => s.GetBlobUrlFromBlobId(blobId))
            .ReturnsAsync(blobId);

            //Act
            var result = await _imageService.GetImageById(request);

            //Assert
            result.ShouldBeRight(r =>
            {
                r.ProductId.Equals("62FA647C-AD54-4BCC-A860-E5A2664B019D");
                r.Id.Equals("testString");
            });
        }

        [Fact]
        public async Task Get_Image_By_ProductId_Fails_NoImage()
        {
            //Arrange
            var request = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D");

            _mockImageRepo
            .Setup(s => s.GetProductImageBlobByProductId(request))
            .ReturnsAsync(AppErrors.ImageNotFoundError);

            //Act
            var result = await _imageService.GetImageById(request);

            //Assert
            result.ShouldBeLeft(r =>
            {
                r.Code.Equals(AppErrors.ImageNotFoundError);
            });
        }

        [Fact]
        public async Task Get_Image_By_ProductId_Fails_NoBlobUrl()
        {
            //Arrange
            var request = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D");
            var blobId = "testString";

            _mockImageRepo
            .Setup(s => s.GetProductImageBlobByProductId(request))
            .ReturnsAsync(new ProductImageBlob { ProductId = new Guid("022e4d43-e585-4eb2-b864-0faab2bf3a4d"), Id = new Guid("b724a2c0-5eae-4727-a5b1-d75156148c80"), BlobId = blobId });

            _mockImageRepo
            .Setup(s => s.GetBlobUrlFromBlobId(blobId))
            .ReturnsAsync(Option<string>.None);

            //Act
            var result = await _imageService.GetImageById(request);

            //Assert
            result.ShouldBeLeft(r =>
            {
                r.Code.Equals(AppErrors.ImageNotFoundError);
            });
        }

        [Fact]
        public async Task Delete_Image_By_ProductId_Succeeds()
        {
            //Arrange
            var request = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D");
            var blobId = "testString";

            _mockImageRepo
            .Setup(s => s.GetProductImageBlobByProductId(request))
            .ReturnsAsync(new ProductImageBlob { ProductId = new Guid("022e4d43-e585-4eb2-b864-0faab2bf3a4d"), Id = new Guid("b724a2c0-5eae-4727-a5b1-d75156148c80"), BlobId = blobId });

            //Act
            var result = await _imageService.DeleteImageById(request);

            //Assert
            result.ShouldBeRight(r =>
            {
                r.IsDefault();
            });
        }

        [Fact]
        public async Task Delete_Image_By_ProductId_Fails_NoBlob()
        {
            //Arrange
            var request = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D");

            _mockImageRepo
            .Setup(s => s.GetProductImageBlobByProductId(request))
            .ReturnsAsync(AppErrors.ImageNotFoundError);

            //Act
            var result = await _imageService.DeleteImageById(request);

            //Assert
            result.ShouldBeLeft(r =>
            {
                r.Code.Equals(AppErrors.ImageNotFoundError);
            });
        }

        [Fact]
        public async Task Delete_Image_By_ProductId_Fails_NoModel()
        {
            //Arrange
            var request = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D");

            _mockImageRepo
            .Setup(s => s.GetProductImageBlobByProductId(request))
            .ReturnsAsync(AppErrors.ImageAlreadyDeletedError);

            //Act
            var result = await _imageService.DeleteImageById(request);

            //Assert
            result.ShouldBeLeft(r =>
            {
                r.Code.Equals(AppErrors.ImageAlreadyDeletedError);
            });
        }
    }
}