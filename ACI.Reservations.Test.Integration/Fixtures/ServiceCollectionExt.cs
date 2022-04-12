using ACI.Reservations.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Reservations.Test.Integration.Fixtures
{
    public static class ServiceCollectionExt
    {
        public static void UseTestDatabase(this IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ReservationDBContext>));

            services.Remove(descriptor ?? throw new InvalidOperationException("Failed to remove default Sql provider"));

            services.AddDbContext<ReservationDBContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();

            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ReservationDBContext>();
            var logger = scopedServices
                .GetRequiredService<ILogger<ReservationTest>>();

            db.Database.EnsureCreated();

            try
            {
                DbSetup.InitializeForTests(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {Message}", ex.Message);
            }
        }
    }
}
