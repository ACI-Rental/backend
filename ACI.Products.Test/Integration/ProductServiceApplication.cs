using System;
using ACI.Products.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ACI.Products.Test.Integration;

public class ProductServiceApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(serviceCollection =>
        {
            serviceCollection.RemoveAll(typeof(ProductContext));
            serviceCollection.AddDbContext<ProductContext>(options =>
            {
                options.UseInMemoryDatabase("IntegrationTestDb");

                var sp = serviceCollection.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ProductContext>();
                var logger = scopedServices.GetRequiredService<ILogger<ProductServiceApplication>>();

                db.Database.EnsureCreated();

                try
                {
                    DbSetup.InitializeForTests(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database with test data. Error: {Message}", ex.Message);
                }
            });
        });
        return base.CreateHost(builder);
    }
}