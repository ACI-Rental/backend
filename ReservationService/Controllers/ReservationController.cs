using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReservationService.DBContexts;
using ReservationService.Models;
using ReservationService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationService.Controllers
{
    /// <summary>
    /// Reservation controller this controller is used for the calls between API and frontend for managing the reservations in the ACI Rental system
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ReservationController : ControllerBase
    {
        /// <summary>
        /// Database context for the reservation service, this is used to make calls to the reservation table
        /// </summary>
        private readonly ReservationServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructor is used for receiving the database context at the creation of the image controller
        /// </summary>
        /// <param name="dbContext">Database context param used for calls to the reservation table</param>
        public ReservationController(ReservationServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all the Reservations from the database
        /// </summary>
        /// <returns>All Reservations in Db</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            var result = await _dbContext.Reservations.ToListAsync();
            return Ok(result);
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
                var result =  await $"https://localhost:44372/api/product/{product.Id}".AllowAnyHttpStatus().GetStringAsync();
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
        /// <returns>All found reservations</returns>
        [HttpGet("{productId}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByProductId(int productId)
        {
            return await _dbContext.Reservations.Where(x => x.ProductId == productId).ToListAsync();
        }
    }
}
