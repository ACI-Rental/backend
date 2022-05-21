using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ACI.Reservations.Domain;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Reservations.Services;
using ACI.Reservations.Services.Interfaces;
using FluentAssertions;
using LanguageExt;
using LanguageExt.UnitTesting;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace ACI.Reservations.Test.Unit
{
    public class ReservationServiceTests
    {
        private static ProductDTO product = new ProductDTO()
        {
            Id = Guid.Parse("66d56f3d-f285-4f11-c9fe-08da17ab56b2"),
            Name = "tv",
            RequiresApproval = false,
            Archived = false,
            CategoryName = "beeldscherm",
        };

        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly IReservationService _reservationService;
        private readonly Mock<HttpMessageHandler> _mockMessageHandler;
        private readonly TestData _testData;
        private readonly Mock<ITimeProvider> _mockTimeProvider;

        public ReservationServiceTests()
        {
            _mockReservationRepository = new Mock<IReservationRepository>();
            _mockProductRepository = new Mock<IProductRepository>();

            var options = Options.Create(new AppConfig() { ApiGatewayBaseUrl = new Uri("https://test.com") });

            _mockMessageHandler = new Mock<HttpMessageHandler>();
            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(product)),
                });

            _mockTimeProvider = new Mock<ITimeProvider>();

            _reservationService = new ReservationService(_mockReservationRepository.Object, options, _mockTimeProvider.Object, _mockProductRepository.Object);

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
        public async Task Execute_ReservationAction_Cancel()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
            };

            var changedReservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
                Cancelled = true,
            };

            _mockReservationRepository
                .Setup(s => s.GetReservationByReservationId(reservation.Id))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.UpdateReservation(reservation))
                .ReturnsAsync(changedReservation);

            // Act
            var result = await _reservationService.ExecuteReservationAction(reservation.Id, ReservationAction.CANCEL);

            // Assert
            result.ShouldBeRight(r =>
            {
                r.Cancelled.Should().Be(true);
            });
        }

        [Fact]
        public async Task Execute_ReservationAction_Pickup()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
            };

            var changedReservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
                PickedUpDate = nextMonday.AddDays(1),
            };

            _mockReservationRepository
                .Setup(s => s.GetReservationByReservationId(reservation.Id))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.UpdateReservation(reservation))
                .ReturnsAsync(changedReservation);

            // Act
            var result = await _reservationService.ExecuteReservationAction(reservation.Id, ReservationAction.PICKUP);

            // Assert
            result.ShouldBeRight(r =>
            {
                r.PickedUpDate.Should().Be(nextMonday.AddDays(1));
            });
        }

        [Fact]
        public async Task Execute_ReservationAction_Return()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
            };

            var changedReservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
                PickedUpDate = nextMonday.AddDays(1),
                ReturnDate = nextMonday.AddDays(2),
            };

            _mockReservationRepository
                .Setup(s => s.GetReservationByReservationId(reservation.Id))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.UpdateReservation(reservation))
                .ReturnsAsync(changedReservation);

            // Act
            var result = await _reservationService.ExecuteReservationAction(reservation.Id, ReservationAction.PICKUP);

            // Assert
            result.ShouldBeRight(r =>
            {
                r.ReturnDate.Should().Be(nextMonday.AddDays(2));
            });
        }

        [Fact]
        public async Task Reserve_Product_Succes()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(2),
            };

            var returnProduct = new Product()
            {
                Id = product.Id,
                Archived = false,
                Name = product.Name,
                RequiresApproval = product.RequiresApproval,
                CategoryName = product.CategoryName,
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(2),
            };

            _mockProductRepository
                .Setup(s => s.GetProductById(ReservationDTO.ProductId))
                .ReturnsAsync(returnProduct);

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(ReservationDTO.ProductId, ReservationDTO.StartDate, ReservationDTO.EndDate))
                .ReturnsAsync(AppErrors.FailedToFindReservation);

            // Act
            var result = await _reservationService.ReserveProduct(ReservationDTO);

            // Assert
            result.ShouldBeRight(r =>
            {
                r.ProductId.Should().Be(product.Id);
            });
        }

        [Fact]
        public async Task Reserve_Product_Fail_No_ProductId()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = Guid.Empty,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(2),
            };

            var reservation = new Reservation()
            {
                ProductId = Guid.Empty,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(2),
            };

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(ReservationDTO.ProductId, ReservationDTO.StartDate, ReservationDTO.EndDate))
                .ReturnsAsync(AppErrors.FailedToFindReservation);

            // Act
            var result = await _reservationService.ReserveProduct(ReservationDTO);

            // Assert
            result.ShouldBeLeft(r =>
            {
                r.Should().Be(AppErrors.ProductNotFoundError);
            });
        }

        [Fact]
        public async Task Reserve_Product_Fail_StartDate_Before_CurrentDate()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday.AddDays(-1),
                EndDate = nextMonday.AddDays(2),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday.AddDays(-1),
                EndDate = nextMonday.AddDays(2),
            };

            _mockTimeProvider
                .Setup(s => s.GetDateTimeNow())
                .Returns(nextMonday);

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(ReservationDTO.ProductId, ReservationDTO.StartDate, ReservationDTO.EndDate))
                .ReturnsAsync(AppErrors.FailedToFindReservation);

            // Act
            var result = await _reservationService.ReserveProduct(ReservationDTO);

            // Assert
            result.ShouldBeLeft(r =>
            {
                r.Should().Be(AppErrors.InvalidStartDate);
            });
        }

        [Fact]
        public async Task Reserve_Product_Fail_EndDate_Before_CurrentDate()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday.AddDays(2),
                EndDate = nextMonday,
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday.AddDays(2),
                EndDate = nextMonday,
            };

            _mockTimeProvider
                .Setup(s => s.GetDateTimeNow())
                .Returns(nextMonday.AddDays(1));

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(ReservationDTO.ProductId, ReservationDTO.StartDate, ReservationDTO.EndDate))
                .ReturnsAsync(AppErrors.FailedToFindReservation);

            // Act
            var result = await _reservationService.ReserveProduct(ReservationDTO);

            // Assert
            result.ShouldBeLeft(r =>
            {
                r.Should().Be(AppErrors.InvalidEndDate);
            });
        }

        [Fact]
        public async Task Reserve_Product_Fail_EndDate_Before_StartDate()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday.AddDays(2),
                EndDate = nextMonday.AddDays(1),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday.AddDays(2),
                EndDate = nextMonday.AddDays(1),
            };

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(ReservationDTO.ProductId, ReservationDTO.StartDate, ReservationDTO.EndDate))
                .ReturnsAsync(AppErrors.FailedToFindReservation);

            // Act
            var result = await _reservationService.ReserveProduct(ReservationDTO);

            // Assert
            result.ShouldBeLeft(r =>
            {
                r.Should().Be(AppErrors.EndDateBeforeStartDate);
            });
        }

        [Fact]
        public async Task Reserve_Product_Fail_StartDate_In_Weekend()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday.AddDays(6),
                EndDate = nextMonday.AddDays(9),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday.AddDays(6),
                EndDate = nextMonday.AddDays(9),
            };

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(ReservationDTO.ProductId, ReservationDTO.StartDate, ReservationDTO.EndDate))
                .ReturnsAsync(AppErrors.FailedToFindReservation);

            // Act
            var result = await _reservationService.ReserveProduct(ReservationDTO);

            // Assert
            result.ShouldBeLeft(r =>
            {
                r.Should().Be(AppErrors.StartDateInWeekend);
            });
        }

        [Fact]
        public async Task Reserve_Product_Fail_EndDate_In_Weekend()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(6),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(6),
            };

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(ReservationDTO.ProductId, ReservationDTO.StartDate, ReservationDTO.EndDate))
                .ReturnsAsync(AppErrors.FailedToFindReservation);

            // Act
            var result = await _reservationService.ReserveProduct(ReservationDTO);

            // Assert
            result.ShouldBeLeft(r =>
            {
                r.Should().Be(AppErrors.EndDateInWeekend);
            });
        }

        [Fact]
        public async Task Reserve_Product_Fail_ExeedsDayLimit()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(9),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(9),
            };

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(ReservationDTO.ProductId, ReservationDTO.StartDate, ReservationDTO.EndDate))
                .ReturnsAsync(AppErrors.FailedToFindReservation);

            // Act
            var result = await _reservationService.ReserveProduct(ReservationDTO);

            // Assert
            result.ShouldBeLeft(r =>
            {
                r.Should().Be(AppErrors.ReservationIsTooLong);
            });
        }

        [Fact]
        public async Task Reserve_Product_Fail_OverlappingReservation()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
            };

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(ReservationDTO.ProductId, ReservationDTO.StartDate, ReservationDTO.EndDate))
                .ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.ReserveProduct(ReservationDTO);

            // Assert
            result.ShouldBeLeft(r =>
            {
                r.Should().Be(AppErrors.ReservationIsOverlapping);
            });
        }

        [Fact]
        public async Task Reserve_Product_Fail_ProductDoesNotExist()
        {
            // Arrange
            var nextMonday = _testData.GetNextMonday();
            var ReservationDTO = new ReservationDTO()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
            };

            var reservation = new Reservation()
            {
                ProductId = product.Id,
                RenterId = Guid.Parse("b57f1be4-30c9-45fd-9472-9abd9d82cad3"),
                StartDate = nextMonday,
                EndDate = nextMonday.AddDays(3),
            };

            _mockProductRepository
                .Setup(s => s.GetProductById(ReservationDTO.ProductId))
                .ReturnsAsync(Option<Product>.None);

            _mockReservationRepository
                .Setup(s => s.CreateReservation(It.IsAny<Reservation>()))
                .ReturnsAsync(reservation);

            _mockReservationRepository
                .Setup(s => s.GetOverlappingReservation(ReservationDTO.ProductId, ReservationDTO.StartDate, ReservationDTO.EndDate))
                .ReturnsAsync(AppErrors.FailedToFindReservation);

            // Act
            var result = await _reservationService.ReserveProduct(ReservationDTO);

            // Assert
            result.ShouldBeLeft(r =>
            {
                r.Should().Be(AppErrors.ProductDoesNotExist);
            });
        }
    }
}