﻿using System;
using System.Threading.Tasks;
using ACI.ImageService.Domain;
using ACI.ImageService.Models;
using LanguageExt;
using LanguageExt.ClassInstances;
using Microsoft.AspNetCore.Http;

namespace ACI.ImageService.Data.Repositories.Interfaces
{
    public interface IImageRepository
    {
        public Task<Either<IError, ProductImageBlob>> AddProductImageBlob(Guid productId, IFormFile file);
        public Task<Either<IError, ProductImageBlob>> GetProductImageBlobById(Guid productId);
        public Task<Option<string>> GetBlobUrlFromBlobId(string blobId);
        public Task<Either<IError, Unit>> DeleteImage(ProductImageBlob blob);
    }
}