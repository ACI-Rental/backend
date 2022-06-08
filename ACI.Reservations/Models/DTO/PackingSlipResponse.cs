using System;

namespace ACI.Reservations.Models.DTO;

public class PackingSlipResponse
{
    public Guid Id { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public string RenterId { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }

    public PackingSlipResponse(Guid id, DateTime startDate, DateTime endDate, string renterId, Guid productId, string productName)
    {
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
        RenterId = renterId;
        ProductId = productId;
        ProductName = productName;
    }

    public static PackingSlipResponse MapFromModel(Reservation model) =>
        new(model.Id, model.StartDate, model.EndDate, model.RenterId, model.ProductId, model.Product.Name);
}