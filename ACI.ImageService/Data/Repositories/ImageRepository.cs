using System;
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
            string test = configuration["AzureBlobStorage:ConnectionStrings:Azurite"];
            _blobContainerClient = new BlobContainerClient("DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=https://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=https://127.0.0.1:10001/devstoreaccount1;TableEndpoint=https://127.0.0.1:10002/devstoreaccount1;", "yeppers1");
            _blobContainerClient.CreateIfNotExists();
            _blobServiceClient = new BlobServiceClient(configuration["AzureBlobStorage:ConnectionStrings:Azurite"]);
            _logger = logger;

        }
        public async Task<Either<IError, ProductImageBlob>> AddProductImageBlob(ProductImageBlob productImageBlob)
        {
            string filepath = "C:/Users/jonah/Pictures/AnonWorld/Sample/PeepoWeird.png";
            BlobClient blob = _blobContainerClient.GetBlobClient("henk");
            await blob.UploadAsync(filepath);
            throw new NotImplementedException("cock??");
        }
    }
}