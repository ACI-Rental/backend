using ACI.Reservations.Models;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Reservations.Services;
using ACI.Reservations.Services.Interfaces;
using FluentAssertions;
using LanguageExt.UnitTesting;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ACI.Reservations.Test.Unit
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly IReservationService _reservationService;

        public ReservationServiceTests()
        {
            _mockReservationRepository = new Mock<IReservationRepository>();

            var options = Options.Create(new AppConfig());

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            _reservationService = new ReservationService(_mockReservationRepository.Object, options, new HttpClient(handlerMock.Object));
        }

        [Fact]
        public async Task Get_Reservations_Succes()
        {
            // Arrange
            var reservations = new List<Reservation>()
            {
                new Reservation()
                {
                    Id = Guid.Parse("30067706-ae02-4bf2-8426-52df52e43684"),
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(3),
                    RenterId = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                },
                new Reservation()
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(3),
                    RenterId = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                },
            };

            _mockReservationRepository
                .Setup(s => s.GetReservations())
                .ReturnsAsync(reservations);

            // Act
            var result = await _reservationService.GetReservations();

            // Assert
            result.ShouldBeRight(r =>
            {
                r.Count.Should().Be(2);
                r[0].Id.Should().Be(Guid.Parse("30067706-ae02-4bf2-8426-52df52e43684"));
            });
        }
    }
}