using ACI.Images.Data.Repositories.Interfaces;
using ACI.Images.Domain.Image;
using ACI.Images.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

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
            //Arrange
            var request = new UploadImageRequest { ProductId = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D"), Image = new FormFile() };

            //Act
            var result = await _imageService.UploadImage(request);

            //Assert
        }

    }
}