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
        public Task<Either<IError, ProductImageBlob>> AddProductImageBlob(ProductImageBlob productImageBlob, IFormFile file);
    }
}