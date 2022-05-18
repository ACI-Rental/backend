using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACI.Reservations.Models;
using ACI.Reservations.Models.DTO;

namespace ACI.Reservations.Test.Unit
{
    internal class TestData
    {
        public List<ReservationDTO> GetReservationData()
        {
            var nextMonday = GetNextMonday();
            return new List<ReservationDTO>()
            {
                new ReservationDTO()
                {
                    Id = Guid.Parse("10067706-ae02-4bf2-8426-52df52e43684"),
                    StartDate = nextMonday,
                    EndDate = nextMonday.AddDays(2),
                    RenterId = Guid.Parse("11067706-ae02-4bf2-8426-52df52e43684"),
                    ProductId = Guid.Parse("11167706-ae02-4bf2-8426-52df52e43684"),
                },
                new ReservationDTO()
                {
                    Id = Guid.Parse("20067706-ae02-4bf2-8426-52df52e43684"),
                    StartDate = nextMonday,
                    EndDate = nextMonday.AddDays(2),
                    RenterId = Guid.Parse("22067706-ae02-4bf2-8426-52df52e43684"),
                    ProductId = Guid.Parse("22267706-ae02-4bf2-8426-52df52e43684"),
                },
                new ReservationDTO()
                {
                    Id = Guid.Parse("30067706-ae02-4bf2-8426-52df52e43684"),
                    StartDate = nextMonday.AddDays(1),
                    EndDate = nextMonday.AddDays(3),
                    RenterId = Guid.Parse("33067706-ae02-4bf2-8426-52df52e43684"),
                    ProductId = Guid.Parse("33367706-ae02-4bf2-8426-52df52e43684"),
                },
                new ReservationDTO()
                {
                    Id = Guid.Parse("40067706-ae02-4bf2-8426-52df52e43684"),
                    StartDate = nextMonday.AddDays(1),
                    EndDate = nextMonday.AddDays(3),
                    RenterId = Guid.Parse("44067706-ae02-4bf2-8426-52df52e43684"),
                    ProductId = Guid.Parse("44467706-ae02-4bf2-8426-52df52e43684"),
                },
                new ReservationDTO()
                {
                    Id = Guid.Parse("50067706-ae02-4bf2-8426-52df52e43684"),
                    StartDate = nextMonday.AddDays(3),
                    EndDate = nextMonday.AddDays(4),
                    RenterId = Guid.Parse("55067706-ae02-4bf2-8426-52df52e43684"),
                    ProductId = Guid.Parse("55567706-ae02-4bf2-8426-52df52e43684"),
                },
            };
        }

        public DateTime GetNextMonday()
        {
            DateTime date = DateTime.Today;
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(1);
            }

            return date;
        }
    }
}
