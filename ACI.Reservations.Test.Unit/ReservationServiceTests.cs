using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Reservations.Services;
using ACI.Reservations.Services.Interfaces;
using FluentAssertions;
using LanguageExt.UnitTesting;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ACI.Reservations.Test.Unit
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly IReservationService _reservationService;
        private readonly Mock<HttpMessageHandler> _mockMessageHandler;
        private readonly TestData _testData;
        private static ProductDTO product = new ProductDTO()
        {
            Id = Guid.Parse("66d56f3d-f285-4f11-c9fe-08da17ab56b2"),
            Name = "tv",
            Description = "tv",
            RequiresApproval = false,
            IsDeleted = false,
            CategoryId = 1,
        };

        public ReservationServiceTests()
        {
            _mockReservationRepository = new Mock<IReservationRepository>();

            var options = Options.Create(new AppConfig() { ApiGatewayBaseUrl = new Uri("https://test.com") });

            _mockMessageHandler = new Mock<HttpMessageHandler>();
            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(product)),
                });

            //var handler = new Mock<HttpMessageHandler>();
            //var client = handler.CreateClient();

            //handler.SetupRequest(HttpMethod.Get, $"{options.Value.ApiGatewayBaseUrl}/products/{productId}")
            //    .ReturnsResponse("{\"id\":\"66d56f3d-f285-4f11-c9fe-08da17ab56b2\",\"name\":\"tv\",\"description\":\"tv\",\"requiresApproval\":false,\"isDeleted\":false,\"categoryId\":1}");

            _reservationService = new ReservationService(_mockReservationRepository.Object, options, new HttpClient(_mockMessageHandler.Object));

            _testData = new TestData();
        }

        [Fact]
        public async Task Get_Reservations_Succes()
        {
            // Arrange
            var reservations = _testData.GetReservationData();

            _mockReservationRepository
                .Setup(s => s.GetReservations())
                .ReturnsAsync(reservations);

            // Act
            var result = await _reservationService.GetReservations();

            // Assert
            result.ShouldBeRight(r =>
            {
                r.Count.Should().Be(5);
                r[0].Id.Should().Be(Guid.Parse("10067706-ae02-4bf2-8426-52df52e43684"));
            });
        }

        [Fact]
        public async Task Get_Reservations_By_Startdate_Succes()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var reservations = _testData.GetReservationData();

            _mockReservationRepository
                .Setup(s => s.GetReservationsByStartDate(nextMonday))
                .ReturnsAsync(reservations.Where(x => x.StartDate == nextMonday).ToList());

            // Act
            var result = await _reservationService.GetReservationsByStartDate(nextMonday);

            // Assert
            result.ShouldBeRight(r =>
            {
                r.Count.Should().Be(2);
                r[0].Id.Should().Be(Guid.Parse("10067706-ae02-4bf2-8426-52df52e43684"));
            });
        }

        [Fact]
        public async Task Get_Reservations_By_EndDate_Succes()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var reservations = _testData.GetReservationData();

            _mockReservationRepository
                .Setup(s => s.GetReservationsByEndDate(nextMonday.AddDays(3)))
                .ReturnsAsync(reservations.Where(x => x.EndDate == nextMonday.AddDays(3)).ToList());

            // Act
            var result = await _reservationService.GetReservationsByEndDate(nextMonday.AddDays(3));

            // Assert
            result.ShouldBeRight(r =>
            {
                r.Count.Should().Be(2);
                r[0].Id.Should().Be(Guid.Parse("30067706-ae02-4bf2-8426-52df52e43684"));
            });
        }

        [Fact]
        public async Task Get_Reservations_By_ProductId_Succes()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var productId = Guid.Parse("11167706-ae02-4bf2-8426-52df52e43684");
            var reservations = _testData.GetReservationData();

            _mockReservationRepository
                .Setup(s => s.GetReservationsByProductId(productId))
                .ReturnsAsync(reservations.Where(x => x.ProductId == productId).ToList());

            // Act
            var result = await _reservationService.GetReservationsByProductId(productId);

            // Assert
            result.ShouldBeRight(r =>
            {
                r.Count.Should().Be(1);
                r[0].ProductId.Should().Be(productId);
            });
        }

        [Fact]
        public async Task Reserve_Product_Succes()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var productReservationDTO = new ProductReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(2),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(2),
            };

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(productReservationDTO.ProductId, productReservationDTO.StartDate, productReservationDTO.EndDate))
                .ReturnsAsync(AppErrors.FailedToFindReservation);

            // Act
            var result = await _reservationService.ReserveProduct(productReservationDTO);

            // Assert
            result.ShouldBeRight(r =>
            {
                r.ProductId.Should().Be(product.Id);
            });
        }
    }
}