using Aci.Images.Test.Integration;
using ACI.Images.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ACI.Images.Test.Integration.Fixtures
{
    public static class ServiceCollectionExt
    {
        public static void UseTestDatabase(this IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ImageContext>));

            services.Remove(descriptor ?? throw new InvalidOperationException("Failed to remove default Sql provider"));

            services.AddDbContext<ImageContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();

            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ImageContext>();
            var logger = scopedServices
                .GetRequiredService<ILogger<ImageTest>>();

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
