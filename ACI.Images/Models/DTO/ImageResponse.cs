
using System;

namespace ACI.Images.Models.DTO
{
    public class ImageResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string BlobUrl { get; set; }
    }
}