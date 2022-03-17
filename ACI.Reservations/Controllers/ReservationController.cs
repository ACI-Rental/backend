﻿using ACI.Reservations.DBContext;
using ACI.Reservations.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Reservations.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationController"/> class.
        /// Constructor is used to define Interfaces.
        /// </summary>
        /// <param name="reservationService">Interface for the ReservationService.</param>
        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Get all the Reservations from the database.
        /// </summary>
        /// <returns>All Reservations in the database.</returns>
        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var result = await _reservationService.GetReservations();
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
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
                return BadRequest("RESERVATION.NO_VALID_DATETIME");
            }

            var result = await _reservationService.GetReservationsByStartDate(startDate);
            if (result.Count > 0)
            {
                return Ok(result);
            }

            return NotFound("RESERVATION.NONE.FOUND");
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
                return BadRequest("RESERVATION.NO_VALID_DATETIME");
            }

            var result = await _reservationService.GetReservationsByEndDate(endDate);
            if (result.Count > 0)
            {
                return Ok(result);
            }

            return NotFound("RESERVATION.NONE.FOUND");
        }

        /// <summary>
        /// Saves a reservation for a product.
        /// </summary>
        /// <param name="reserveProductModel">Model containing a list of product models.</param>
        /// <returns>Returns BadRequest with a list of errors, returns Ok if successful</returns>
        [HttpPost("reserveproducts")]
        public async Task<IActionResult> ReserveProducts(ReserveProductModel reserveProductModel)
        {
            var revervations = new List<Reservation>();
            var productModelsErrorList = new List<KeyValuePair<ProductModel, string>>();
            if (reserveProductModel.ProductModels == null || reserveProductModel.ProductModels.Count <= 0)
            {
                return BadRequest("PRODUCT.RESERVE.NO_PRODUCTS");
            }


            foreach (var product in reserveProductModel.ProductModels)
            {
                // Later we will add time slots
                var tempStartDate = product.StartDate.AddHours(23 - product.StartDate.Hour);
                tempStartDate = tempStartDate.AddMinutes(59 - product.StartDate.Minute);
                tempStartDate = tempStartDate.AddSeconds(58 - product.StartDate.Second);

                var tempEndDate = product.EndDate.AddHours(23 - product.EndDate.Hour);
                tempEndDate = tempEndDate.AddMinutes(59 - product.EndDate.Minute);
                tempEndDate = tempEndDate.AddSeconds(59 - product.EndDate.Second);

                if (product.Id <= 0)
                {
                    productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.PRODUCT_NO_ID"));
                }
                if (tempStartDate < DateTime.Now)
                {
                    productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.PRODUCT_INVALID_STARTDATE"));
                }
                if (tempEndDate < DateTime.Now)
                {
                    productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.PRODUCT_INVALID_ENDDATE"));
                }
                if (tempEndDate < tempStartDate)
                {
                    productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.PRODUCT_ENDDATE_BEFORE_STARTDATE"));
                }
                if (product.StartDate.DayOfWeek == DayOfWeek.Saturday || product.StartDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.STARTDATE_IN_WEEKEND"));
                }
                if (product.EndDate.DayOfWeek == DayOfWeek.Saturday || product.EndDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.ENDDATE_IN_WEEKEND"));
                }
                int weekenddays = AmountOfWeekendDays(product.StartDate, product.EndDate);
                double totalamountofdays = (product.EndDate - product.StartDate).TotalDays - weekenddays;
                if (totalamountofdays > 5)
                {
                    productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.RESERVATION_TIME_TO_LONG"));
                }
                // https://stackoverflow.com/questions/13513932/algorithm-to-detect-overlapping-periods
                var foundReservation = await _dbContext.Reservations.Where(x => x.StartDate <= product.EndDate && product.StartDate < x.EndDate).FirstOrDefaultAsync();
                if (foundReservation != null)
                {
                    productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.PRODUCT_ALREADY_RESERVED_IN_PERIOD"));
                }
                var result = await $"{_config.Value.ApiGatewayBaseUrl}/api/product/flat/{product.Id}".AllowAnyHttpStatus().GetStringAsync();
                if (string.IsNullOrWhiteSpace(result))
                {
                    productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.PRODUCT_NOT_FOUND"));
                }
                else
                {
                    var foundProduct = JsonConvert.DeserializeObject<Product>(result);
                    if (foundProduct == null)
                    {
                        productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.PRODUCT_NOT_FOUND"));
                    }
                    else
                    {
                        if (foundProduct.ProductState != ProductState.AVAILABLE)
                        {
                            productModelsErrorList.Add(new KeyValuePair<ProductModel, string>(product, "PRODUCT.RESERVE.PRODUCT_NOT_AVAILABLE"));
                        }
                        if (productModelsErrorList.Count <= 0)
                        {
                            //TODO: Add RenterID with JWT claims
                            var reservation = new Reservation()
                            {
                                ProductId = product.Id,
                                StartDate = product.StartDate,
                                EndDate = product.EndDate,
                                IsApproved = foundProduct.RequiresApproval ? false : null,
                            };
                            revervations.Add(reservation);
                        }
                    }
                }
            }
            if (productModelsErrorList.Count > 0)
            {
                return BadRequest(productModelsErrorList);
            }
            foreach (var item in revervations)
            {
                _dbContext.Reservations.Add(item);
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Get all reservations with similar startdate for a single page
        /// </summary>
        /// <returns>List of all similar reservations</returns>
        [HttpGet("similar/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetSimilarReservations(int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                return BadRequest("RESERVATION.INCORRECT_INDEX");
            }

            if (pageSize <= 0)
            {
                return BadRequest("RESERVATION.INCORRECT_INDEX");
            }

            var page = new ReservationOverviewPage();

            var reservations = await _dbContext.Reservations.ToListAsync();
            var similarReservations = new List<List<Reservation>>();

            for (int i = 0; i < reservations.Count; i++)
            {
                var mergedReservations = new List<Reservation>();

                mergedReservations.AddRange(reservations.Where(x => x.StartDate.Date == reservations[i].StartDate.Date && x.RenterId == reservations[i].RenterId).ToList());

                reservations.RemoveAll(x => x.StartDate.Date == reservations[i].StartDate.Date && x.RenterId == reservations[i].RenterId && reservations[i].Id != x.Id);

                similarReservations.Add(mergedReservations);
            }

            page.TotalReservationCount = similarReservations.Count();

            // Last page calculation goes wrong if the totalcount is 0
            // also no point in trying to get 0 products from DB
            if (page.TotalReservationCount == 0)
            {
                page.CurrentPage = 0;
                page.Reservations = new List<List<Reservation>>(0);
                return Ok(page);
            }

            // Calculate how many pages there are, given the current pageSize
            int lastPage = (int)Math.Ceiling((double)page.TotalReservationCount / pageSize) - 1;

            //pageIndex below 0 is nonsensical, bringing the value to closest sane value
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }

            // Use last page if requested page is higher
            page.CurrentPage = Math.Min(pageIndex, lastPage);

            page.Reservations = similarReservations.Skip(page.CurrentPage * pageSize).Take(pageSize).ToList();
            return Ok(page);
        }

        /// <summary>
        /// Counts the amount of weekend days between two dates.
        /// </summary>
        /// <param name="startDate">The starting date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>The amount days that are weekend days found</returns>
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

        /// <summary>
        /// Get all reservations that are linked to a product
        /// </summary>
        /// <param name="productId">productId used to find the reservations</param>
        /// <param name="excludeHistory">Whether past reservation should be excluded from the results</param>
        /// <returns>All found reservations</returns>
        [HttpGet("{productId}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByProductId(int productId, [FromQuery(Name = "excludeHistory")] bool excludeHistory = true)
        {
            var query = _dbContext.Reservations.Where(x => x.ProductId == productId);
            if (excludeHistory)
            {
                query = query.Where(x => x.EndDate >= DateTime.Today);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// A action that can be done on a reservation, updating the database
        /// Actions that can be done are: Cancel, Pickup and Return
        /// </summary>
        /// <param name="reservationActionModel">Action Object with Reservation Id and Action number</param>
        /// <returns>Ok</returns>
        [HttpPost("reservationaction")]
        public async Task<IActionResult> ReservationActionCall(ReservationActionModel reservationActionModel)
        {
            if (reservationActionModel == null)
            {
                return BadRequest("RESERVATION.ACTION.INVALID.CALL");
            }

            if (reservationActionModel.ReservationId < 0)
            {
                return BadRequest("RESERVATION.ACTION.INVALID.ID");
            }

            var result = _dbContext.Reservations.SingleOrDefault(b => b.Id == reservationActionModel.ReservationId);
            if (result != null)
            {
                switch (reservationActionModel.Action)
                {
                    case ReservationAction.CANCEL:
                        throw new NotImplementedException();
                    case ReservationAction.PICKUP:
                        result.PickedUpDate = DateTime.Now;
                        break;
                    case ReservationAction.RETURN:
                        result.ReturnDate = DateTime.Now;
                        break;
                    default:
                        return BadRequest("RESERVATION.ACTION.INVALID.ACTION");
                }
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}