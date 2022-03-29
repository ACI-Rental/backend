using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Reservations.Services;
using ACI.Reservations.Services.Interfaces;
using Moq;
using System;
using System.Net.Http;
using LanguageExt.UnitTesting;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ACI.Reservations.Test.Unit
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;

        private readonly IReservationService _reservationService;

        public ReservationServiceTests()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _reservationService = new ReservationService(_reservationRepositoryMock.Object, new HttpClient());
        }

        [Fact]
        public async Task CreateReservationTest()
        {
            // Arrange
            var monday = GetNextMonday();
            var newReservation = new Reservation()
            {
                Id = Guid.NewGuid(),
                StartDate = monday,
                EndDate = monday.AddDays(2),
                RenterId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
            };

            var newReservationDTO = new ProductReservationDTO()
            {
                StartDate = monday,
                EndDate = monday.AddDays(2),
                RenterId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
            };

            _reservationRepositoryMock
                .Setup(s => s.CreateReservation(newReservation))
                .ReturnsAsync(newReservation);

            // Act
            var result = await _reservationService.CreateReservation(newReservationDTO);

            // Assert
            result.ShouldBeRight(r =>
            {
                r.StartDate.Should().Be(newReservation.StartDate);
                r.EndDate.Should().Be(newReservation.EndDate);
                r.RenterId.Should().Be(newReservation.RenterId);
                r.ProductId.Should().Be(newReservation.ProductId);
            });
        }

        [Fact]
        public async Task GetReservationsTest()
        {
            // Arrange
            var reservations = GetReservations();

            _reservationRepositoryMock.Setup(s => s.GetReservations()).ReturnsAsync(reservations);

            // Act
            var result = await _reservationService.GetReservations();

            // Assert
            result.ShouldBeRight(r =>
            {
                r.Count.Should().Be(2);
            });
        }

        private DateTime GetNextMonday()
        {
            DateTime date = DateTime.Today;
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(1);
            }
            return date;
        }

        public List<Reservation> GetReservations()
        {
            var monday = GetNextMonday();
            return new List<Reservation>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    StartDate = monday,
                    EndDate = monday.AddDays(2),
                    RenterId = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    StartDate = monday,
                    EndDate = monday.AddDays(3),
                    PickedUpDate = monday.AddDays(1),
                    ReturnDate = monday.AddDays(2),
                    RenterId = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                },
            };
        }
    }
}