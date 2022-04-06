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
using LanguageExt;
using ACI.Images.Models;

namespace ACI.Images.Test.Unit
{
    public class ImageServiceTests
    {
        private readonly Mock<IImageRepository> _mockImageRepo;
        private readonly IImageService _imageService;
        IConfiguration _configuration;

        public ImageServiceTests()
        {
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
            .ReturnsAsync(new ProductImageBlob { ProductId = request.ProductId}); //ID & blobID?

            //Act
            var result = await _imageService.UploadImage(request);

            //Assert
            result.ShouldBeRight(r =>
            {
                r.ProductId.Should().Be(request.ProductId);
            });
        }

    }
}