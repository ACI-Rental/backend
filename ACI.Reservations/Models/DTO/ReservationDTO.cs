using System;
using System.Collections.Generic;

namespace ACI.Reservations.Models.DTO
{
    public class ReservationDTO
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? PickedUpDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? RenterId { get; set; }
        public string? RenterName { get; set; }
        public string? RenterEmail { get; set; }
        public string? ReviewerId { get; set; }
        public bool? IsApproved { get; set; }
        public Guid ProductId { get; set; }
        public ProductDTO? Product { get; set; }
        public ReservationStatus Status { get; set; }

        public ReservationDTO()
        {
        }

        public ReservationDTO(Guid id, DateTime startDate, DateTime endDate, DateTime? pickedUpDate, DateTime? returnDate, string renterId, string? renterName, string? renterEmail, string? reviewerId, bool? isApproved, Guid productId, Product product, ReservationStatus status)
        {
            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            PickedUpDate = pickedUpDate;
            ReturnDate = returnDate;
            RenterId = renterId;
            RenterName = renterName;
            RenterEmail = renterEmail;
            ReviewerId = reviewerId;
            IsApproved = isApproved;
            ProductId = productId;
            Product = product == null ? null : ProductDTO.MapFromModel(product);
            Status = status;
        }

        public static ReservationDTO MapFromModel(Reservation model)
            => new(
                model.Id,
                model.StartDate,
                model.EndDate,
                model.PickedUpDate,
                model.ReturnDate,
                model.RenterId,
                model.RenterName,
                model.RenterEmail,
                model.ReviewerId,
                model.IsApproved,
                model.ProductId,
                model.Product,
                model.Status);

        public static List<ReservationDTO> MapFromList(List<Reservation> list)
            => list.ConvertAll(x => MapFromModel(x));
    }
}
