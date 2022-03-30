using ACI.Reservations.Models.DTO;

namespace ACI.Reservations.Models
{
    public class ProductDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public bool RequiresApproval { get; set; }

        public int CategoryId { get; set; }

        public virtual ProductCategoryDTO Category { get; set; } = null!;
    }
}
