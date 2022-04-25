using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACI.Images.Data.Repositories.Interfaces;
using ACI.Images.Domain;
using ACI.Images.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LanguageExt;
using LanguageExt.Pretty;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ACI.Images.Data.Repositories
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
            _blobContainerClient.CreateIfNotExists(PublicAccessType.Blob);
            _blobServiceClient = new BlobServiceClient(configuration["ConnectionStrings:Azurite"]);
            _logger = logger;
            _context = context;
        }
        
        public async Task<Either<IError, ProductImageBlob>> AddProductImageBlob(Guid productId, IFormFile image)
        {
            var productIdExists = await _context.Images.AnyAsync(x => x.ProductId.Equals(productId));

            if (productIdExists)
            {
                _logger.LogInformation("Adding image {image} failed with error {Error}", productId, AppErrors.ProductIdAlreadyExistsError);
                return AppErrors.ProductIdAlreadyExistsError;
            }

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

        public Either<IError, ProductImageBlob> GetProductImageBlobByProductId(Guid productId)
        {
            var image = _context.Images.FirstOrDefault(x => x.ProductId == productId);

            if (image == null) return AppErrors.ImageNotFoundError;

            return image;
        }
        
        public Option<string> GetBlobUrlFromBlobId(string blobId)
        {
            BlobClient blob = _blobContainerClient.GetBlobClient(blobId);

            var blobUri = blob.Uri.ToString();

            if (blobUri == string.Empty) return Option<string>.None;

            return blobUri;

        }

        public async Task<Either<IError, Unit>> DeleteImage(ProductImageBlob blob)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blob.BlobId);

            blobClient.Delete();

            _context.Images.Remove(blob);
            await _context.SaveChangesAsync();

            return Unit.Default;
        }

    }
}