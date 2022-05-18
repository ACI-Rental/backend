using System;
using ACI.Reservations.Models.DTO;

namespace ACI.Reservations.Models
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool RequiresApproval { get; set; }
        public bool Archived { get; set; }
        public int CategoryId { get; set; }
        public int CatalogPosition { get; set; }

        public ProductDTO()
        {

        }

        public ProductDTO(Guid id, string name, string description, string location, bool requiresApproval, bool archived, int categoryId, int catalogPosition)
        {
            Id = id;
            Name = name;
            Description = description;
            Location = location;
            RequiresApproval = requiresApproval;
            Archived = archived;
            CategoryId = categoryId;
            CatalogPosition = catalogPosition;
        }

        public static ProductDTO MapFromModel(Product model)
            => new(
                model.Id,
                model.Name,
                model.Description,
                model.Location,
                model.RequiresApproval,
                model.Archived,
                model.CategoryId,
                model.CatalogPosition);
    }
}
