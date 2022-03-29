using System;
using System.Collections.Generic;
using ACI.Reservations.Models;

namespace ACI.Reservations.Test.Unit
{
    public class TestData
    {
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

        private DateTime GetNextMonday()
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
