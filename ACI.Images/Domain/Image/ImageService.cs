using System;
using System.IO;
using System.Threading.Tasks;
using ACI.ImageService.Data.Repositories.Interfaces;
using ACI.ImageService.Models;
using ACI.ImageService.Models.DTO;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ACI.ImageService.Domain.Image
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<ImageService> _imageServce;
        private readonly string _urlPrefix;

        public ImageService(IImageRepository imageRepository, ILogger<ImageService> imageServce, IConfiguration configuration)
        {
            _imageRepository = imageRepository;
            _imageServce = imageServce;
            _urlPrefix = configuration["AzureBlobStorage:UrlPrefix"];
        }

        public async Task<Either<IError, ImageResponse>> UploadImage(UploadImageRequest request)
        {
            var result = await _imageRepository.AddProductImageBlob(request.ProductId, request.Image);

            return result.Map(blobResponse =>
            {
                return new ImageResponse() { 
                    Id = blobResponse.Id, 
                    ProductId = blobResponse.ProductId, 
                    BlobUrl = $"{_urlPrefix}/{blobResponse.BlobId}" };
            });
        }
    }
}