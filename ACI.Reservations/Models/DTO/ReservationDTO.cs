using System;

namespace ACI.Reservations.Models.DTO
{
    public class ReservationDTO
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? PickedUpDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public Guid RenterId { get; set; }
        public Guid? ReviewerId { get; set; }
        public bool? IsApproved { get; set; }
        public Guid ProductId { get; set; }
        public ProductDTO? Product { get; set; }
        public bool Cancelled { get; set; } = false;

        public ReservationDTO()
        {

        }

        public ReservationDTO(Guid id, DateTime startDate, DateTime endDate, DateTime? pickedUpDate, DateTime? returnDate, Guid renterId, Guid? reviewerId, bool? isApproved, Guid productId, Product product, bool cancelled)
        {
            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            PickedUpDate = pickedUpDate;
            ReturnDate = returnDate;
            RenterId = renterId;
            ReviewerId = reviewerId;
            IsApproved = isApproved;
            ProductId = productId;
            Product = Product == null ? null : ProductDTO.MapFromModel(product);
            Cancelled = cancelled;
        }

        public static ReservationDTO MapFromModel(Reservation model)
            => new(
                model.Id,
                model.StartDate,
                model.EndDate,
                model.PickedUpDate,
                model.ReturnDate,
                model.RenterId,
                model.ReviewerId,
                model.IsApproved,
                model.ProductId,
                model.Product,
                model.Cancelled);
    }
}
