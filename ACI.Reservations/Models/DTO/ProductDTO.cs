using ACI.Reservations.Models.DTO;

namespace ACI.Reservations.Models
{
    public class ProductDTO
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool RequiresApproval { get; }
        public bool IsDeleted { get; }
        public int CategoryId { get; }
    }
}
