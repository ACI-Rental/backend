using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationService.Controllers;
using ReservationService.DBContexts;
using ReservationService.Models;
using ReservationService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Flurl.Http.Testing;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace ReservationService.Tests.UnitTests
{
    public class ReservationTests : IDisposable
    {
        private readonly ReservationController _controller;
        private readonly ReservationServiceDatabaseContext _context;
        private bool disposedValue;

        public ReservationTests()
        {
            var options = new DbContextOptionsBuilder<ReservationServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryReservationDb").Options;

            _context = new ReservationServiceDatabaseContext(options);
            _controller = new ReservationController(_context, Options.Create(new AppConfig() { ApiGatewayBaseUrl = "http://fake-url.com" }));
        }

        [Fact]
        private async Task ReserveProducts_ShouldReturnStartDateBeforeCurrentDateError()
        {
            var monday = GetNextMonday();
            var model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            var pm1 = new ProductModel { Id = 6, StartDate = monday.AddDays(-14), EndDate = monday.AddDays(-13) };
            model.ProductModels.Add(pm1);
            var product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using var httpTest = new HttpTest();

            httpTest.RespondWith(serializedObject);
            var result = await _controller.ReserveProducts(model);
            var errorList = GetErrorList(result);
            Assert.Equal(2, errorList.Count);
            Assert.Equal(pm1, errorList[0].Key);
            Assert.Equal("PRODUCT.RESERVE.PRODUCT_INVALID_STARTDATE", errorList[0].Value);
            Assert.Equal("PRODUCT.RESERVE.PRODUCT_INVALID_ENDDATE", errorList[1].Value);
        }

        [Fact]
        private async Task ReserveProducts_ShouldReturnEndDateBeforeCurrentDateError()
        {
            var monday = GetNextMonday();
            var model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            var pm1 = new ProductModel { Id = 6, StartDate = monday, EndDate = monday.AddDays(-3) };
            model.ProductModels.Add(pm1);
            var product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using var httpTest = new HttpTest();
            httpTest.RespondWith(serializedObject);
            var result = await _controller.ReserveProducts(model);
            var errorList = GetErrorList(result);
            Assert.Single(errorList);
            Assert.Equal(pm1, errorList[0].Key);
            Assert.Equal("PRODUCT.RESERVE.PRODUCT_ENDDATE_BEFORE_STARTDATE", errorList[0].Value);
        }

        [Fact]
        private async Task ReserveProducts_ShouldReturnEndDateBeforeStartDateError()
        {
            var monday = GetNextMonday();
            var model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            var pm1 = new ProductModel { Id = 6, StartDate = monday.AddDays(7), EndDate = monday.AddDays(4) };
            model.ProductModels.Add(pm1);
            var product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using var httpTest = new HttpTest();
            httpTest.RespondWith(serializedObject);
            var result = await _controller.ReserveProducts(model);
            var errorList = GetErrorList(result);
            Assert.Single(errorList);
            Assert.Equal(pm1, errorList[0].Key);
            Assert.Equal("PRODUCT.RESERVE.PRODUCT_ENDDATE_BEFORE_STARTDATE", errorList[0].Value);
        }

        [Fact]
        private async Task ReserveProducts_ShouldReturnStartDayWeekendError()
        {
            var weekendDay = GetNextWeekendDay();
            var model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            var pm1 = new ProductModel { Id = 6, StartDate = weekendDay, EndDate = weekendDay.AddDays(2) };
            model.ProductModels.Add(pm1);
            var product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using var httpTest = new HttpTest();
            httpTest.RespondWith(serializedObject);
            var result = await _controller.ReserveProducts(model);
            var errorList = GetErrorList(result);
            Assert.Single(errorList);
            Assert.Equal(pm1, errorList[0].Key);
            Assert.Equal("PRODUCT.RESERVE.STARTDATE_IN_WEEKEND", errorList[0].Value);
        }

        [Fact]
        private async Task ReserveProducts_ShouldReturnEndDayWeekendError()
        {
            var weekendDay = GetNextWeekendDay();
            var model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            var pm1 = new ProductModel { Id = 6, StartDate = DateTime.Now, EndDate = weekendDay };
            model.ProductModels.Add(pm1);
            var product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using var httpTest = new HttpTest();
            httpTest.RespondWith(serializedObject);
            var result = await _controller.ReserveProducts(model);
            var errorList = GetErrorList(result);
            Assert.Single(errorList);
            Assert.Equal(pm1, errorList[0].Key);
            Assert.Equal("PRODUCT.RESERVE.ENDDATE_IN_WEEKEND", errorList[0].Value);
        }

        [Fact]
        private async Task ReserveProducts_ShouldReturnReservationTimeToLongError()
        {
            var monday = GetNextMonday();
            var model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            var pm1 = new ProductModel { Id = 6, StartDate = monday, EndDate = monday.AddDays(8) };
            model.ProductModels.Add(pm1);
            var product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using var httpTest = new HttpTest();
            httpTest.RespondWith(serializedObject);
            var result = await _controller.ReserveProducts(model);
            var errorList = GetErrorList(result);
            Assert.Single(errorList);
            Assert.Equal(pm1, errorList[0].Key);
            Assert.Equal("PRODUCT.RESERVE.RESERVATION_TIME_TO_LONG", errorList[0].Value);
        }

        [Fact]
        private async Task ReserveProducts_ShouldReturnReservationPeriodAlreadyTakenError()
        {
            var monday = GetNextMonday();
            var model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            var pm1 = new ProductModel { Id = 6, StartDate = monday, EndDate = monday.AddDays(3) };
            model.ProductModels.Add(pm1);
            var product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            var reservation = new Reservation() { StartDate = monday, EndDate = monday.AddDays(5) };
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                var errorList = GetErrorList(result);
                Assert.Single(errorList);
                Assert.Equal(pm1, errorList[0].Key);
                Assert.Equal("PRODUCT.RESERVE.PRODUCT_ALREADY_RESERVED_IN_PERIOD", errorList[0].Value);
            }
            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
        }

        [Fact]
        private async Task ReserveProducts_ShouldReturnNoProductFoundError()
        {
            var monday = GetNextMonday();
            var model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            var pm1 = new ProductModel { Id = 6, StartDate = monday, EndDate = monday.AddDays(1) };
            model.ProductModels.Add(pm1);
            var product = new Product() { Id = 1, ProductState = ProductState.UNAVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using var httpTest = new HttpTest();
            httpTest.RespondWith(serializedObject);
            var result = await _controller.ReserveProducts(model);
            var errorList = GetErrorList(result);
            Assert.Single(errorList);
            Assert.Equal(pm1, errorList[0].Key);
            Assert.Equal("PRODUCT.RESERVE.PRODUCT_NOT_AVAILABLE", errorList[0].Value);
        }

        [Fact]
        private async Task ReserveProducts_ShouldReturnNoProductFoundMultipleProductsError()
        {
            var monday = GetNextMonday();
            var model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            var pm1 = new ProductModel { Id = 6, StartDate = monday, EndDate = monday.AddDays(4) };
            var pm2 = new ProductModel { Id = 6, StartDate = monday.AddDays(8), EndDate = monday.AddDays(10) };
            model.ProductModels.Add(pm1);
            model.ProductModels.Add(pm2);
            var product = new Product() { Id = 1, ProductState = ProductState.UNAVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using var httpTest = new HttpTest();
            httpTest.RespondWith(serializedObject);
            var result = await _controller.ReserveProducts(model);
            var errorList = GetErrorList(result);
            Assert.Equal(2, errorList.Count);
            Assert.Equal(pm1, errorList[0].Key);
            Assert.Equal(pm2, errorList[1].Key);
            Assert.Equal("PRODUCT.RESERVE.PRODUCT_NOT_AVAILABLE", errorList[0].Value);
            Assert.Equal("PRODUCT.RESERVE.PRODUCT_NOT_AVAILABLE", errorList[1].Value);
        }

        [Fact]
        private async Task ReserveProducts_ShouldReturnOkForSuccessfulReservation()
        {
            var monday = GetNextMonday();
            var model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            var pm1 = new ProductModel { Id = 6, StartDate = monday, EndDate = monday.AddDays(4) };
            model.ProductModels.Add(pm1);
            var product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                Assert.IsType<OkResult>(result);
            }
            Assert.Equal(1, _context.Reservations.Count());
        }

        [Fact]
        private async Task GetReservationsByProductId_ShouldReturnCurrentReservation()
        {
            _context.Reservations.Add(new Reservation() { Id = 1, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1), IsApproved = true, RenterId = 1, ProductId = 1 });
            await _context.SaveChangesAsync();

            var reservation = (await _controller.GetReservationsByProductId(1, false));
            Assert.Single(reservation.Value);
        }

        [Fact]
        private async Task GetReservationsByProductId_ShouldNotReturnPastReservation()
        {
            _context.Reservations.Add(new Reservation() { Id = 1, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(-1), IsApproved = true, RenterId = 1, ProductId = 1 });
            await _context.SaveChangesAsync();

            var reservation = (await _controller.GetReservationsByProductId(1, true));
            Assert.Empty(reservation.Value);
        }

        [Fact]
        private async Task GetReservationsByProductId_ShouldOnlyReturnCurrentReservation()
        {
            _context.Reservations.Add(new Reservation() { Id = 1, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1), IsApproved = true, RenterId = 1, ProductId = 1 });
            _context.Reservations.Add(new Reservation() { Id = 2, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(-1), IsApproved = true, RenterId = 1, ProductId = 1 });
            await _context.SaveChangesAsync();

            var reservation = (await _controller.GetReservationsByProductId(1, true));
            Assert.Single(reservation.Value);
        }

        [Fact]
        private async Task GetReservationsByProductId_ShouldReturnPastReservation()
        {
            _context.Reservations.Add(new Reservation() { Id = 1, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(-1), IsApproved = true, RenterId = 1, ProductId = 1 });
            await _context.SaveChangesAsync();

            var reservation = (await _controller.GetReservationsByProductId(1, false));
            Assert.Single(reservation.Value);
        }

        /// <summary>
        /// Gets error from a badrequest error for the Reserve Products tests
        /// </summary>
        /// <param name="result">The IActionResult from the controller call</param>
        /// <returns></returns>
        private List<KeyValuePair<ProductModel, string>> GetErrorList(IActionResult result)
        {
            var objectResult = Assert.IsType<BadRequestObjectResult>(result);

            var errorIEnumerable = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<ProductModel, string>>>(objectResult.Value);
            var errorList = errorIEnumerable.ToList();
            return errorList;
        }

        /// <summary>
        /// Gets the next Saturday or Sunday as a datetime
        /// </summary>
        /// <returns>Datetime object of de next weekend day</returns>
        private DateTime GetNextWeekendDay()
        {
            DateTime date = DateTime.Today;
            while (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }
            return date;
        }

        /// <summary>
        /// Gets the next Mondayas a datetime
        /// </summary>
        /// <returns>Datetime object of de next weekend day</returns>
        private DateTime GetNextMonday()
        {
            DateTime date = DateTime.Today;
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(1);
            }
            return date;
        }

        /// <summary>
        /// Disposes resource
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && _context != null)
                {
                    _context.Reservations.RemoveRange(_context.Reservations.ToList());
                    _context.SaveChanges();
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
