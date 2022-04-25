using System;
using System.Linq;
using ACI.Products.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ACI.Products.Test.Integration.Fixtures;

public static class ServiceCollectionExt
{
    public static void UseTestDatabase(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType ==
                 typeof(DbContextOptions<ProductContext>));

        services.Remove(descriptor ?? throw new InvalidOperationException("Failed to remove default Sql provider"));

        services.AddDbContext<ProductContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDbForTesting");
        });

        var sp = services.BuildServiceProvider();

        using var scope = sp.CreateScope();

        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<ProductContext>();
        var logger = scopedServices
            .GetRequiredService<ILogger<ProductTest>>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        try
        {
            DbSetup.Clean(db);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {Message}", ex.Message);
        }
    }
}