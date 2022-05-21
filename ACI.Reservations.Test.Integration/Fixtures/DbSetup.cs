using System;
using System.Collections.Generic;
using System.Linq;
using ACI.Reservations.DBContext;
using ACI.Reservations.Models;
using Bogus;

namespace ACI.Reservations.Test.Integration.Fixtures
{
    public class DbSetup
    {
        public const int ReservationsTotal = 98;
        public const int ProductsTotal = 49;
        private static readonly Random Random = new Random();

        public static void InitializeForTests(ReservationDBContext db)
        {
            db.Reservations.AddRange(GetReservations());
            db.Products.AddRange(GetProducts());
            db.SaveChanges();
        }

        public static void Clean(ReservationDBContext db)
        {
            db.Reservations.RemoveRange(db.Reservations.ToList());
            db.Products.RemoveRange(db.Products.ToList());
            db.SaveChanges();

            InitializeForTests(db);
        }

        public static List<Reservation> GetReservations(int amount = ReservationsTotal)
        {
            var list = new Faker<Reservation>()
                .RuleFor(r => r.StartDate, GetNextMonday().AddDays(7))
                .RuleFor(r => r.EndDate, f => GetNextMonday().AddDays(f.Random.Int(8, 11)))
                .RuleFor(r => r.RenterId, f => f.Random.Guid())
                .RuleFor(r => r.ProductId, f => f.Random.Guid())
                .Generate(amount);

            list.Add(new Reservation() { StartDate = GetNextMonday(), EndDate = GetNextMonday().AddDays(2), RenterId = Guid.NewGuid(), ProductId = Guid.Parse("70661e4b-a4f5-47e8-8c80-5b2c7ab959ff") });
            list.Add(new Reservation() { Id = Guid.Parse("03b0a851-93b7-4397-a64e-e3d7e6f8f891"), StartDate = GetNextMonday(), EndDate = GetNextMonday().AddDays(2), RenterId = Guid.NewGuid(), ProductId = Guid.Parse("70661e4b-a4f5-47e8-8c80-5b2c7ab959ff") });

            return list;
        }

        public static List<Product> GetProducts(int amount = ProductsTotal)
        {
            var list = new Faker<Product>()
                .RuleFor(r => r.Name, f => f.Random.Word())
                .RuleFor(r => r.Archived, false)
                .RuleFor(r => r.RequiresApproval, false)
                .RuleFor(r => r.CategoryName, f => f.Random.Word())
                .Generate(amount);

            list.Add(new Product() { Id = Guid.Parse("4b45abe7-bd89-4645-8dc1-6f842c5ab7af"), Name = "tv", Archived = false, RequiresApproval = false, CategoryName = "beeldscherm" });
            return list;
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
    }
}
