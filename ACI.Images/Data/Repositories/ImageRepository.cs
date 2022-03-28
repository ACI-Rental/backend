using System;
using System.IO;
using System.Threading.Tasks;
using ACI.ImageService.Data.Repositories.Interfaces;
using ACI.ImageService.Domain;
using ACI.ImageService.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ACI.ImageService.Data.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly ILogger<ImageRepository> _logger;
        private readonly ImageContext _context;
        
        public ImageRepository(IConfiguration configuration, ILogger<ImageRepository> logger, ImageContext context)
        {
            _blobContainerClient = new BlobContainerClient(configuration["ConnectionStrings:Azurite"], configuration["AzureBlobStorage:Containers:ProductImages"]);
            _blobContainerClient.CreateIfNotExists();
            _blobServiceClient = new BlobServiceClient(configuration["ConnectionStrings:Azurite"]);
            _logger = logger;
            _context = context;
        }
        
        public async Task<Either<IError, ProductImageBlob>> AddProductImageBlob(Guid productId, IFormFile image)
        {
            string fileExtension = Path.GetExtension(image.FileName);
            string blobName = $"{Guid.NewGuid()}{fileExtension}";

            var productImageBlob = new ProductImageBlob()
            {
                ProductId = productId,
                BlobId = blobName
            };

            BlobClient blob = _blobContainerClient.GetBlobClient(productImageBlob.BlobId);
            await blob.UploadAsync(image.OpenReadStream());
            
            await _context.Images.AddAsync(productImageBlob);
            await _context.SaveChangesAsync();

            return productImageBlob;
        }

        public async Task<Option<ProductImageBlob>> GetImage(Guid productId)
        {
            var image = _context.Images.FirstOrDefault(x => x.ProductId == productId);
            if (image == null) return Option<ProductImageBlob>.None;

            BlobClient blob = _blobContainerClient.GetBlobClient(image.BlobId);

            BlobDownloadInfo download = await blob.DownloadToAsync();

            return download.
        }
    }
}