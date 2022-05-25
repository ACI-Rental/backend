using System;
using System.Threading.Tasks;
using ACI.Reservations.Domain;
using ACI.Reservations.Messaging.Consumers;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using ACI.Reservations.Services.Interfaces;
using ACI.Shared.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACI.Reservations.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController : BaseController
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationsController"/> class.
        /// Constructor is used to define Interfaces.
        /// </summary>
        /// <param name="reservationService">Interface for the ReservationService.</param>
        /// <param name="logger">This is the logger that logs application actions.</param>
        /// <param name="bus">This is the messaging bus.</param>
        /// <param name="createdConsumer">This is the consumer.</param>
        public ReservationsController(IReservationService reservationService, ILogger<ReservationsController> logger, IBus bus, ProductCreatedConsumer createdConsumer)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all the Reservations from the database.
        /// </summary>
        /// <returns>All Reservations in the database.</returns>
        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var result = await _reservationService.GetReservations();

            return result
                .Right<IActionResult>(value => Ok(value))
                .Left(err => BadRequest(err));
        }

        /// <summary>
        /// Get all reservations with the same StartDate as specified in the parameter.
        /// </summary>
        /// <param name="startDate">The parameter to get reservations with the same StartDate.</param>
        /// <returns>A List with reservations that have the specified StartDate.</returns>
        [HttpGet("bystartdate/{startDate}")]
        public async Task<IActionResult> GetReservationsWithSimilarStartDate(DateTime startDate)
        {
            if (startDate == DateTime.MinValue)
            {
                return BadRequest(AppErrors.InvalidStartDate);
            }

            var result = await _reservationService.GetReservationsByStartDate(startDate);

            return result
                .Right<IActionResult>(value => Ok(value))
                .Left(err => NotFound(err));
        }

        /// <summary>
        /// Get all reservations with the same EndDate as specified in the parameter.
        /// </summary>
        /// <param name="endDate">The parameter to get reservations with the same EndDate.</param>
        /// <returns>A List with reservations that have the specified EndDate.</returns>
        [HttpGet("byenddate/{endDate}")]
        public async Task<IActionResult> GetReservationsWithSimilarEndDate(DateTime endDate)
        {
            if (endDate == DateTime.MinValue)
            {
                return BadRequest(AppErrors.InvalidEndDate);
            }

            var result = await _reservationService.GetReservationsByEndDate(endDate);

            return result
                .Right<IActionResult>(value => Ok(value))
                .Left(err => NotFound(err));
        }

        /// <summary>
        /// Creates a new reservation to be added to the database.
        /// </summary>
        /// <param name="productReservation">This parameter contains the ProductId, RenterId, StartDate, and EndDate of the new reservation.</param>
        /// <returns>A status 200 if the reservation is created succesfully of a Status 400 if an error occured while creating the new reservation.</returns>
        [HttpPost("reserveproduct")]
        public async Task<IActionResult> ReserveProduct([FromBody] ReservationDTO productReservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = GetUser();

            productReservation.RenterId = user.Id;
            _logger.LogInformation("Creating new Reservation {productReservation}", productReservation);

            var result = await _reservationService.ReserveProduct(productReservation);

            return result
                .Right<IActionResult>(Ok)
                .Left(err => BadRequest(err));
        }

        /// <summary>
        /// Get all reservations that contain the same product.
        /// </summary>
        /// <param name="productId">This parameter is used to get the reservations from the database.</param>
        /// <returns>A List of reservations with the productId.</returns>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetReservationsByProductId(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                return BadRequest();
            }

            var result = await _reservationService.GetReservationsByProductId(productId);

            return result
                .Right<IActionResult>(value => Ok(value))
                .Left(err => NotFound(err));
        }

        // TODO: Delete this when replaced by individual Cancel/Pickup/Return endpoints
        [HttpPost("action")]
        public async Task<IActionResult> ExecuteReservationAction([FromBody] ReservationActionDTO reservationActionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (reservationActionDTO.ReservationId == Guid.Empty || reservationActionDTO.ReservationAction > 3 || reservationActionDTO.ReservationAction < 1)
            {
                return BadRequest();
            }

            var action = (ReservationAction)Enum.ToObject(typeof(ReservationAction), reservationActionDTO.ReservationAction);

            var result = await _reservationService.ExecuteReservationAction(reservationActionDTO.ReservationId, action);

            return result
                .Right<IActionResult>(Ok)
                .Left(err => BadRequest(err));
        }
    }
}
