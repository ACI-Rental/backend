using ACI.Reservations.DBContext;
using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Reservations.Services.Interfaces;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Newtonsoft.Json;

namespace ACI.Reservations.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly HttpClient _httpClient;

        public ReservationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
            _httpClient = new HttpClient();
        }

        public async Task<Either<IError, List<Reservation>>> GetReservations()
        {
            return await _reservationRepository.GetReservations();
        }

        public async Task<Either<IError, List<Reservation>>> GetReservationsByStartDate(DateTime startDate)
        {
            return await _reservationRepository.GetReservationsByStartDate(startDate);
        }

        public async Task<Either<IError, List<Reservation>>> GetReservationsByEndDate(DateTime endDate)
        {
            return await _reservationRepository.GetReservationsByEndDate(endDate);
        }

        public async Task<Either<IError, List<Reservation>>> GetReservationsByProductId(Guid productId)
        {
            return await _reservationRepository.GetReservationsByProductId(productId);
        }

        public async Task<Either<IError, Reservation>> ExecuteReservationAction(Guid reservationId, ReservationAction action)
        {
            var reservation = await _reservationRepository.GetReservationByReservationId(reservationId);

            if (reservation.GetType() == typeof(IError))
            {
                return reservation;
            }

            var reservationToChange = reservation.ValueUnsafe();

            switch (action)
            {
                case ReservationAction.CANCEL:
                    reservationToChange.Cancelled = true;
                    break;
                case ReservationAction.PICKUP:
                    reservationToChange.PickedUpDate = DateTime.Now;
                    break;
                case ReservationAction.RETURN:
                    reservationToChange.ReturnDate = DateTime.Now;
                    break;
                default:
                    return AppErrors.InvalidReservationAction;
            }

            return await _reservationRepository.UpdateReservation(reservationToChange);
        }

        public async Task<Either<IError, Reservation>> ReserveProduct(ProductReservationDTO productReservationDTO)
        {
            var result = await ValidateReservationData(productReservationDTO);
            if (result.IsSome)
            {
                return result.ValueUnsafe();
            }

            var productResult = await _httpClient.GetAsync($"https://localhost:5019/products/{productReservationDTO.ProductId}");
            var content = await productResult.Content.ReadAsStringAsync();
            if (!productResult.IsSuccessStatusCode)
            {
                return AppErrors.ProductNotFoundError;
            }

            var product = JsonConvert.DeserializeObject<ProductDTO>(content.ToString());
            var reservation = new Reservation()
            {
                ProductId = productReservationDTO.ProductId,
                RenterId = productReservationDTO.RenterId,
                StartDate = productReservationDTO.StartDate,
                EndDate = productReservationDTO.EndDate,
            };

            if (product != null && product.RequiresApproval && product.Id != Guid.Empty)
            {
                reservation.IsApproved = false;
            }

            return await _reservationRepository.CreateReservation(reservation);
        }

        private async Task<Option<IError>> ValidateReservationData(ProductReservationDTO productReservationDTO)
        {
            if (productReservationDTO.ProductId == Guid.Empty)
            {
                return AppErrors.ProductNotFoundError;
            }

            if (productReservationDTO.StartDate < DateTime.Now)
            {
                return AppErrors.InvalidStartDate;
            }

            if (productReservationDTO.EndDate < DateTime.Now)
            {
                return AppErrors.InvalidEndDate;
            }

            if (productReservationDTO.EndDate < productReservationDTO.StartDate)
            {
                return AppErrors.EndDateBeforeStartDate;
            }

            if (productReservationDTO.StartDate.DayOfWeek == DayOfWeek.Saturday || productReservationDTO.StartDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return AppErrors.StartDateInWeekend;
            }

            if (productReservationDTO.EndDate.DayOfWeek == DayOfWeek.Saturday || productReservationDTO.EndDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return AppErrors.EndDateInWeekend;
            }

            int weekenddays = AmountOfWeekendDays(productReservationDTO.StartDate, productReservationDTO.EndDate);
            double totalamountofdays = (productReservationDTO.EndDate - productReservationDTO.StartDate).TotalDays - weekenddays;
            if (totalamountofdays > 5)
            {
                return AppErrors.ReservationIsTooLong;
            }

            var reservationResult = await _reservationRepository.GetOverlappingReservation(productReservationDTO.ProductId, productReservationDTO.StartDate, productReservationDTO.EndDate);
            if (reservationResult.ValueUnsafe() != null && reservationResult.ValueUnsafe().Id != Guid.Empty)
            {
                return AppErrors.ReservationIsOverlapping;
            }

            // TODO: get product from messagebroker to check if it exists.
            var productResult = await _httpClient.GetAsync($"https://localhost:5019/products/{productReservationDTO.ProductId}");
            if (!productResult.IsSuccessStatusCode)
            {
                return AppErrors.ProductDoesNotExist;
            }

            return Option<IError>.None;
        }

        private int AmountOfWeekendDays(DateTime startDate, DateTime endDate)
        {
            int amountOfWeekendDays = 0;
            for (DateTime i = startDate; i < endDate; i = i.AddDays(1))
            {
                if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
                {
                    amountOfWeekendDays++;
                }
            }

            return amountOfWeekendDays;
        }
    }
}
