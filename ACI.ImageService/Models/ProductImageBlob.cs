using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACI.ImageService.Models
{
    public class ProductImageBlob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public Guid BlobId { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
    }
}