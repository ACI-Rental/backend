using System.ComponentModel.DataAnnotations;

namespace ACI.Products.Models.DTO
{
    public class ProductArchiveRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public bool Archived { get; set; }
    }
}
