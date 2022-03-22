using System.Threading.Tasks;
using ACI.ImageService.Models.DTO;
using LanguageExt;

namespace ACI.ImageService.Domain.Image
{
    public interface IImageService
    {
        public Task<Either<IError, ImageResponse>> UploadImage(UploadImageRequest request);
    }
}