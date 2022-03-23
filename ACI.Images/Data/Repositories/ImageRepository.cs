using System;
using System.IO;
using System.Threading.Tasks;
using ACI.ImageService.Data.Repositories.Interfaces;
using ACI.ImageService.Domain;
using ACI.ImageService.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ACI.ImageService.Data.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly ILogger<ImageRepository> _logger;
        
        public ImageRepository(IConfiguration configuration, ILogger<ImageRepository> logger)
        {
            _blobContainerClient = new BlobContainerClient(configuration["AzureBlobStorage:ConnectionStrings:Azurite"], configuration["AzureBlobStorage:Containers:ProductImages"]);
            _blobContainerClient.CreateIfNotExists();
            _blobServiceClient = new BlobServiceClient(configuration["AzureBlobStorage:ConnectionStrings:Azurite"]);
            _logger = logger;
        }
        
        public async Task<Either<IError, ProductImageBlob>> AddProductImageBlob(ProductImageBlob productImageBlob)
        {
            string filepath = "C:/Users/jonah/Pictures/how-it-looks-when-someone-with-an-android-accidentally-turns-18866052.png";

            var ext = Path.GetExtension(filepath);
            BlobClient blob = _blobContainerClient.GetBlobClient("henk.png");
            await blob.UploadAsync(filepath);
            throw new NotImplementedException("cock??");
        }
    }
}