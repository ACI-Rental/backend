using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Images.Models
{
    public class ProductImageBlob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Required]
        public Guid Id { get; set; }

        // The blobs name is the ID ex: f5eac4f1-fa02-491b-8340-1947d6557558.png
        [Required]
        public string BlobId { get; set; }

        [FromBody]
        public Guid ProductId { get; set; }
    }
}