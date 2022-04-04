using System;
using System.Threading.Tasks;
using ACI.ImageService.Models.DTO;
using LanguageExt;

namespace ACI.ImageService.Domain.Image
{
    public interface IImageService
    {
        public Task<Either<IError, ImageResponse>> UploadImage(UploadImageRequest request);
        public Task<Either<IError, ImageResponse>> GetImageById(Guid productId);
        public Task<Either<IError, Unit>> DeleteImageById(Guid productId);
    }
}