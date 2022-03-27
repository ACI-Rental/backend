using System;
using System.IO;
using System.Threading.Tasks;
using ACI.ImageService.Data.Repositories.Interfaces;
using ACI.ImageService.Models;
using ACI.ImageService.Models.DTO;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACI.ImageService.Domain.Image
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<ImageService> _imageServce;
        
        public ImageService(IImageRepository  imageRepository, ILogger<ImageService> imageServce)
        {
            _imageRepository = imageRepository;
            _imageServce = imageServce;
        }
        
        public async Task<Either<IError, ImageResponse>> UploadImage(UploadImageRequest request)
        {
            
            string fileExtension = Path.GetExtension(request.Image.FileName);
            string blobName = $"{Guid.NewGuid()}{fileExtension}";
            
            var newBlob = new ProductImageBlob()
            {
                ProductId = request.ProductId,
                BlobId = blobName
            };
            
            await _imageRepository.AddProductImageBlob(newBlob, request.Image);
            throw new NotImplementedException("Implement LanguageExt return shit");
        }
    }
}