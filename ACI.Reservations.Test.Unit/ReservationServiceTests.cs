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
        private readonly TestData _testData;

        public ReservationServiceTests()
        {
            _mockReservationRepository = new Mock<IReservationRepository>();

            var options = Options.Create(new AppConfig());

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            _reservationService = new ReservationService(_mockReservationRepository.Object, options, new HttpClient(handlerMock.Object));

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
                .ReturnsAsync(reservations);

            // Act
            var result = await _reservationService.GetReservationsByStartDate(nextMonday);

            // Assert
            result.ShouldBeRight(r =>
            {
                r.Count.Should().Be(5);
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
                .Setup(s => s.GetReservationsByStartDate(nextMonday.AddDays(3)))
                .ReturnsAsync(reservations);

            // Act
            var result = await _reservationService.GetReservationsByStartDate(nextMonday.AddDays(3));

            // Assert
            result.ShouldBeRight(r =>
            {
                r.Count.Should().Be(5);
                r[0].Id.Should().Be(Guid.Parse("10067706-ae02-4bf2-8426-52df52e43684"));
            });
        }
    }
}