﻿using System;
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
        
        public ImageRepository(IConfiguration configuration, ILogger<ImageRepository> logger)
        {
            _blobContainerClient = new BlobContainerClient(configuration["AzureBlobStorage:ConnectionStrings:Azurite"], configuration["AzureBlobStorage:Containers:ProductImages"]);
            _blobContainerClient.CreateIfNotExists();
            _blobServiceClient = new BlobServiceClient(configuration["AzureBlobStorage:ConnectionStrings:Azurite"]);
            _logger = logger;
        }
        
        public async Task<Either<IError, ProductImageBlob>> AddProductImageBlob(ProductImageBlob productImageBlob, IFormFile Image)
        {
            BlobClient blob = _blobContainerClient.GetBlobClient(productImageBlob.BlobId);
            await blob.UploadAsync(Image.OpenReadStream());
            
            // TODO: Upload data to SQL db
            throw new NotImplementedException("database insert gebeuren");
        }
    }
}