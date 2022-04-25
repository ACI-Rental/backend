using System;
using System.Threading.Tasks;
using ACI.Images.Domain;
using ACI.Images.Models;
using LanguageExt;
using LanguageExt.ClassInstances;
using Microsoft.AspNetCore.Http;

namespace ACI.Images.Data.Repositories.Interfaces
{
    public interface IImageRepository
    {
        public Task<Either<IError, ProductImageBlob>> AddProductImageBlob(Guid productId, IFormFile file);
        public Either<IError, ProductImageBlob> GetProductImageBlobByProductId(Guid productId);
        public Option<string> GetBlobUrlFromBlobId(string blobId);
        public Task<Either<IError, Unit>> DeleteImage(ProductImageBlob blob);
    }
}