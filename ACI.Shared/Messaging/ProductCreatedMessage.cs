using System;

namespace ACI.Shared.Messaging
{
    public class ProductCreatedMessage
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool RequiresApproval { get; set; }
        public bool IsDeleted { get; set; }
        public int CategoryId { get; set; }
    }
}