using System;
using System.IO;
using System.Threading.Tasks;
using ACI.ImageService.Data.Repositories.Interfaces;
using ACI.ImageService.Models;
using ACI.ImageService.Models.DTO;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ACI.ImageService.Domain.Image
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<ImageService> _logger;
        private readonly string _urlPrefix;

        public ImageService(IImageRepository imageRepository, ILogger<ImageService> logger, IConfiguration configuration)
        {
            _imageRepository = imageRepository;
            _logger = logger;
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

        public async Task<Option<ImageResponse>> GetImageById(Guid productId)
        {
            var result = await _imageRepository.GetProductImageBlobById(productId);

            var blobUri = await _imageRepository.GetBlobUrlFromBlobId(result.ValueUnsafe().BlobId);

            return result.Map(productImageBlob =>
            {
                return new ImageResponse()
                {
                    Id = productImageBlob.Id,
                    ProductId = productImageBlob.ProductId,
                    BlobUrl = blobUri.ValueUnsafe()
                };
            });
        }

        public async Task<Either<IError, Unit>> DeleteImageById(Guid productId)
        {
            var productImageBlob = await _imageRepository.GetProductImageBlobById(productId);

            if (productImageBlob.IsNone)
            {
                _logger.LogInformation("Deleting productimageblob {ProductId} failed with error {Error}", productId, AppErrors.ImageNotFoundError);
                return AppErrors.ImageNotFoundError;
            }

            var blob = productImageBlob.ValueUnsafe();

            if (blob.IsDeleted) 
            {
                _logger.LogInformation("Product {ProductId} is already deleted", productId);
                return AppErrors.ImageAlreadyDeletedError;
            }

            await _imageRepository.DeleteImage(blob);
            return Unit.Default;
        }

    }
}