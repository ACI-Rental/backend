using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;
using ACI.Reservations.Test.Integration.Fixtures;
using FluentAssertions;
using Xunit;

namespace ACI.Reservations.Test.Integration
{
    public class ReservationTest : IClassFixture<ReservationAppFactory>
    {
        private readonly HttpClient _apiClient;

        public ReservationTest(ReservationAppFactory factory)
        {
            _apiClient = factory.CreateClient();
        }

        public static DateTime GetNextMonday()
        {
            DateTime date = DateTime.Today;
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(1);
            }

            return date;
        }

        [Fact]
        public async void Get_All_Reservations()
        {
            // Arrange

            // Act
            var response = await _apiClient.GetAllReservations();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var reservations = await response.Content.ReadFromJsonAsync<List<Reservation>>();

            reservations.Should().NotBeNull();
            reservations.Count.Should().Be(100);
        }

        [Fact]
        public async void Get_Reservations_By_StartDate()
        {
            // Arrange
            var monday = GetNextMonday();

            // Act
            var response = await _apiClient.GetReservationsByStartDate(monday.AddDays(7));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var reservations = await response.Content.ReadFromJsonAsync<List<Reservation>>();

            reservations.Should().NotBeNull();
            reservations.Count.Should().Be(97);
        }

        [Fact]
        public async void Get_Reservations_By_StartDate_To_Find_No_Reservations()
        {
            // Arrange
            var monday = GetNextMonday();

            // Act
            var response = await _apiClient.GetReservationsByStartDate(monday.AddDays(20));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void Get_Reservations_By_StartDate_To_Fail()
        {
            // Arrange
            var monday = DateTime.MinValue;

            // Act
            var response = await _apiClient.GetReservationsByStartDate(monday);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void Get_Reservations_By_EndDate()
        {
            // Arrange
            var endDate = GetNextMonday().AddDays(2);

            // Act
            var response = await _apiClient.GetReservationsByEndDate(endDate);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var reservations = await response.Content.ReadFromJsonAsync<List<Reservation>>();

            reservations.Should().NotBeNull();
            reservations[0].EndDate.Date.Should().Be(endDate);
        }

        [Fact]
        public async void Get_Reservations_By_EndDate_To_Find_No_Reservations()
        {
            // Arrange
            var monday = GetNextMonday();

            // Act
            var response = await _apiClient.GetReservationsByEndDate(monday.AddDays(20));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void Get_Reservations_By_EndDate_To_Fail()
        {
            // Arrange
            var monday = DateTime.MinValue;

            // Act
            var response = await _apiClient.GetReservationsByEndDate(monday);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void Get_Reservations_By_ProductId()
        {
            // Arrange
            var productId = Guid.Parse("4b45abe7-bd89-4645-8dc1-6f842c5ab7ef");

            // Act
            var response = await _apiClient.GetReservationsByProductId(productId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var reservations = await response.Content.ReadFromJsonAsync<List<Reservation>>();

            reservations.Should().NotBeNull();
            reservations[0].ProductId.Should().Be(productId);
        }

        [Fact]
        public async void Get_Reservations_By_ProductId_No_Reservation_Found()
        {
            // Arrange
            var productId = Guid.Parse("72661e4b-a4f5-47e8-8c80-5b2c7ab959ff");

            // Act
            var response = await _apiClient.GetReservationsByProductId(productId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void Get_Reservations_By_ProductId_To_Fail()
        {
            // Arrange
            var productId = Guid.Empty;

            // Act
            var response = await _apiClient.GetReservationsByProductId(productId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void Execute_ReservationAction_Cancel()
        {
            // Arrange
            var reservationActionDTO = new ReservationActionDTO() { ReservationId = Guid.Parse("03b0a851-93b7-4397-a64e-e3d7e6f8f891"), ReservationAction = (int)ReservationAction.CANCEL };

            // Act
            var response = await _apiClient.ExecuteReservationAction(reservationActionDTO);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void Execute_ReservationAction_Pickup()
        {
            // Arrange
            var reservationActionDTO = new ReservationActionDTO() { ReservationId = Guid.Parse("03b0a851-93b7-4397-a64e-e3d7e6f8f891"), ReservationAction = (int)ReservationAction.PICKUP };

            // Act
            var response = await _apiClient.ExecuteReservationAction(reservationActionDTO);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void Execute_ReservationAction_Return()
        {
            // Arrange
            var reservationActionDTO = new ReservationActionDTO() { ReservationId = Guid.Parse("03b0a851-93b7-4397-a64e-e3d7e6f8f891"), ReservationAction = (int)ReservationAction.RETURN };

            // Act
            var response = await _apiClient.ExecuteReservationAction(reservationActionDTO);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void Execute_ReservationAction_Invalid_Model()
        {
            // Arrange
            var reservationActionDTO = new ReservationActionDTO() { ReservationAction = (int)ReservationAction.RETURN };

            // Act
            var response = await _apiClient.ExecuteReservationAction(reservationActionDTO);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void Execute_ReservationAction_Invalid_Action()
        {
            // Arrange
            var reservationActionDTO = new ReservationActionDTO() { ReservationId = Guid.Parse("03b0a851-93b7-4397-a64e-e3d7e6f8f891"), ReservationAction = 5 };

            // Act
            var response = await _apiClient.ExecuteReservationAction(reservationActionDTO);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void Reserve_Product_Success()
        {
            // Arrange
            var newReservation = new ReservationDTO()
            {
                ProductId = Guid.Parse("4b45abe7-bd89-4645-8dc1-6f842c5ab7af"),
                StartDate = GetNextMonday().AddDays(3),
                EndDate = GetNextMonday().AddDays(4),
            };

            // Act
            var response = await _apiClient.ReserveProduct(newReservation);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void Reserve_Product_Fail_Overlapping()
        {
            // Arrange
            var newReservation = new ReservationDTO()
            {
                ProductId = Guid.Parse("4b45aba7-bd89-4645-8dc1-6f842c5ab7af"),
                StartDate = GetNextMonday().AddDays(1),
                EndDate = GetNextMonday().AddDays(2),
            };
            await _apiClient.ReserveProduct(newReservation);

            // Act
            var response = await _apiClient.ReserveProduct(newReservation);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
