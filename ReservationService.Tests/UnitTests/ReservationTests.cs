using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationService.Controllers;
using ReservationService.DBContexts;
using ReservationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReservationService.Tests.UnitTests
{
    public class ReservationTests
    {
        [Fact]
        public async Task GetReservations_WhenCalled_ReturnListOfReservations()
        {
            var options = new DbContextOptionsBuilder<ReservationServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryReservationDb").Options;

            var context = new ReservationServiceDatabaseContext(options);
            SeedReservationInMemoryDatabaseWithData(context);
            var controller = new ReservationController(context);
            var result = await controller.GetReservations();

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var reservations = Assert.IsAssignableFrom<IEnumerable<Reservation>>(objectresult.Value);

            Assert.Equal(3, reservations.Count());
            Assert.Equal(1, reservations.ElementAt(0).Id);
            Assert.Equal(2, reservations.ElementAt(1).Id);
            Assert.Equal(5, reservations.ElementAt(2).Id);
        }

        private static void SeedReservationInMemoryDatabaseWithData(ReservationServiceDatabaseContext context)
        {
            var data = new List<Reservation>
                {
                    new Reservation { Id = 1 },
                    new Reservation { Id = 2 },
                    new Reservation { Id = 5 }
                };
            context.Reservations.AddRange(data);
            context.SaveChanges();

        }
    }
}
