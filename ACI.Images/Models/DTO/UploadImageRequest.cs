using System;
using System.ComponentModel.DataAnnotations;
using ACI.ImageService.Helpers;
using Microsoft.AspNetCore.Http;

namespace ACI.ImageService.Models.DTO
{
    public class UploadImageRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [AllowedExtensions(".jpg,.gif,.jpeg,.png")]
        public IFormFile Image { get; set; }
    }
}