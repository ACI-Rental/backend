using System;
using System.ComponentModel.DataAnnotations;

namespace ACI.ImageService.Models.DTO
{
    public class UploadImageRequest
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
    }
}