using System;
using ACI.Products.Data;
using ACI.Products.Data.Repositories;
using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Domain.Category;
using ACI.Products.Domain.Product;
using ACI.Products.Messaging;
using ACI.Reservations.Models;
using ACI.Shared;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting ACI.Products Microservice");

try
{
    Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

void Run()
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddAciLogging();
    
    // Add services to the container.
    builder.Services.AddControllers();

    builder.Services.AddDbContext<ProductContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

    // Core services
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();

    builder.Services.AddScoped<IProductMessaging, ProductMessaging>();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    
    // Bind app settings to configurations
    builder.Services
        .AddOptions<AppConfig>()
        .Bind(builder.Configuration.GetSection(AppConfig.Key))
        .ValidateDataAnnotations()
        .ValidateOnStart();
    
    builder.Services.AddMassTransit(x =>
    {
        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
        {
            // config.UseHealthCheck(provider);
            config.Host(new Uri(builder.Configuration.GetSection(AppConfig.Key)["RabbitMQBaseUrl"]), h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
        }));
    });
    
    builder.Services.AddMassTransitHostedService();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ProductContext>();
        context.Database.EnsureCreated();
    }

    app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}

public partial class Program
{
    // Expose the Program class for use with WebApplicationFactory<T>
}