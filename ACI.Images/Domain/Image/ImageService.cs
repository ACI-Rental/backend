using System;
using ACI.Images.Data.Repositories.Interfaces;
using ACI.Images.Models.DTO;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;

namespace ACI.Images.Domain.Image
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

        public async Task<Either<IError, ImageResponse>> GetImageById(Guid productId)
        {
            var result = await _imageRepository.GetProductImageBlobById(productId);

            if (result.IsLeft)
            {
                _logger.LogInformation("Getting productimageblob {ProductId} failed with error {Error}", productId, AppErrors.ImageNotFoundError);
                return AppErrors.ImageNotFoundError;
            }

            var blobUri = await _imageRepository.GetBlobUrlFromBlobId(result.ValueUnsafe().BlobId);

            if (blobUri.IsNone)
            {
                _logger.LogInformation("Getting blobUri {ProductId} failed with error {Error}", productId, AppErrors.ImageNotFoundError);
                return AppErrors.ImageNotFoundError;
            }

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

            if (productImageBlob.IsNull())
            {
                _logger.LogInformation("Deleting productimageblob {ProductId} failed with error {Error}", productId, AppErrors.ImageNotFoundError);
                return AppErrors.ImageNotFoundError;
            }

            var blob = productImageBlob.ValueUnsafe();

            if (blob.IsNull()) 
            {
                _logger.LogInformation("Productimageblob {ProductId} is already deleted", productId);
                return AppErrors.ImageAlreadyDeletedError;
            }

            await _imageRepository.DeleteImage(blob);
            return Unit.Default;
        }

    }
}