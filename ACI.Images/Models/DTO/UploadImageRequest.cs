using System.ComponentModel.DataAnnotations;
using ACI.Images.Helpers;

namespace ACI.Images.Models.DTO
{
    public class UploadImageRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [AllowedExtensions(".png")]
        public IFormFile Image { get; set; }
    }
}