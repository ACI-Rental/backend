using System;
namespace ACI.Shared.Messaging;

public class ProductUpdatedMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool RequiresApproval { get; set; }
    public bool Archived { get; set; }
    public string CategoryName { get; set; }
}