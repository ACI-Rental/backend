using System;
using System.Threading.Tasks;
using ACI.Images.Models.DTO;
using LanguageExt;

namespace ACI.Images.Domain.Image
{
    public interface IImageService
    {
        public Task<Either<IError, ImageResponse>> UploadImage(UploadImageRequest request);
        public Either<IError, ImageResponse> GetImageById(Guid productId);
        public Task<Either<IError, Unit>> DeleteImageById(Guid productId);
    }
}