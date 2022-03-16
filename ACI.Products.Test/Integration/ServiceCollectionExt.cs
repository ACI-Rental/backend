using System;
using System.Linq;
using ACI.Products.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ACI.Products.Test.Integration;

public static class ServiceCollectionExt
{
    public static void UseTestDatabase(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType ==
                 typeof(DbContextOptions<ProductContext>));

        services.Remove(descriptor);

        services.AddDbContext<ProductContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDbForTesting");
        });

        var sp = services.BuildServiceProvider();

        using (var scope = sp.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ProductContext>();
            var logger = scopedServices
                .GetRequiredService<ILogger<ProductTest>>();

            db.Database.EnsureCreated();

            try
            {
                DbSetup.InitializeForTests(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "An error occurred seeding the database with test messages. Error: {Message}",
                    ex.Message);
            }
        }
    }
}