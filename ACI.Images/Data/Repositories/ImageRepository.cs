using ACI.Images.Data.Repositories.Interfaces;
using ACI.Images.Domain;
using ACI.Images.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

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
            
            string fileExtension = Path.GetExtension(image.FileName);
            string blobName = $"{productId}{fileExtension}";

            var productImageBlob = new ProductImageBlob()
            {
                ProductId = productId,
                BlobId = blobName
            };
            
            if (productIdExists)
            {
                await DeleteImage(productImageBlob);
            }
            
            BlobClient blob = _blobContainerClient.GetBlobClient(productImageBlob.BlobId);
            await blob.UploadAsync(image.OpenReadStream());
            
            await _context.Images.AddAsync(productImageBlob);
            await _context.SaveChangesAsync();

            return productImageBlob;
        }

        public Either<IError, ProductImageBlob> GetProductImageBlobByProductId(Guid productId)
        {
            var image = _context.Images.FirstOrDefault(x => x.ProductId == productId);

            if (image == null)
            {
                return AppErrors.ImageNotFoundError;
            }

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

            await blobClient.DeleteAsync();

            var productImageBlob = await _context.Images.FirstOrDefaultAsync(x => x.ProductId == blob.ProductId);
            
            if (productImageBlob != null) _context.Images.Remove(productImageBlob);
            await _context.SaveChangesAsync();

            return Unit.Default;
        }

    }
}