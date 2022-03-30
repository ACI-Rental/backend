using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using ACI.Reservations.Services.Interfaces;
using LanguageExt.UnsafeValueAccess;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Reservations.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationController"/> class.
        /// Constructor is used to define Interfaces.
        /// </summary>
        /// <param name="reservationService">Interface for the ReservationService.</param>
        /// <param name="logger">This is the logger that logs application actions.</param>
        public ReservationController(IReservationService reservationService, ILogger<ReservationController> logger)
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
        [HttpGet("bystartdate/{datetime}")]
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
        [HttpGet("byenddate/{datetime}")]
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
        public async Task<IActionResult> ReserveProduct([FromBody] ProductReservationDTO productReservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _logger.LogInformation("Creating new Reservation {productReservation}", productReservation);

            var result = await _reservationService.CreateReservation(productReservation);

            return result
                .Right<IActionResult>(Ok)
                .Left(err => BadRequest(err));
        }

        /// <summary>
        /// Get all reservations that contain the same product.
        /// </summary>
        /// <param name="productId">This parameter is used to get the reservations from the database.</param>
        /// <returns>A List of reservations with the productId.</returns>
        [HttpGet("Reservation/{productId}")]
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

        [HttpPost("reservationaction/{reservationId}/{reservationAction}")]
        public async Task<IActionResult> ExecuteReservationAction(Guid reservationId, int reservationAction)
        {
            if (reservationId == Guid.Empty || reservationAction > 2 || reservationAction < 0)
            {
                return BadRequest();
            }

            var action = (ReservationAction)Enum.ToObject(typeof(ReservationAction), reservationAction);

            var result = await _reservationService.ExecuteReservationAction(reservationId, action);

            return result
                .Right<IActionResult>(Ok)
                .Left(err => BadRequest(err));
        }
    }
}
