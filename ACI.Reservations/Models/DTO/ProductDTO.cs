using System;
using ACI.Reservations.Models.DTO;

namespace ACI.Reservations.Models
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool RequiresApproval { get; set; }
        public bool Archived { get; set; }
        public string CategoryName { get; set; }

        public ProductDTO()
        {

        }

        public ProductDTO(Guid id, string name,bool requiresApproved, bool archived, string categoryName)
        {
            Id = id;
            Name = name;
            RequiresApproval = requiresApproved;
            Archived = archived;
            CategoryName = categoryName;
        }

        public static ProductDTO MapFromModel(Product model)
            => new(
                model.Id,
                model.Name,
                model.RequiresApproval,
                model.Archived,
                model.CategoryName);
    }
}
