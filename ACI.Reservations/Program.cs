using System;
using ACI.Reservations.DBContext;
using ACI.Reservations.Domain;
using ACI.Reservations.Messaging;
using ACI.Reservations.Messaging.Consumers;
using ACI.Reservations.Models;
using ACI.Reservations.Repositories;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Reservations.Services;
using ACI.Reservations.Services.Interfaces;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ACI.Shared;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting ACI.Reservations microservice");

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

    // Bind app settings to configurations
    builder.Services
        .AddOptions<AppConfig>()
        .Bind(builder.Configuration.GetSection(AppConfig.Key))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<ProductCreatedConsumer>();
        x.AddConsumer<ProductDeletedConsumer>();
        
        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
        {
            // config.UseHealthCheck(provider);
            config.Host(new Uri(builder.Configuration.GetSection(AppConfig.Key)["RabbitMqBaseUrl"]), h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
            
            config.ReceiveEndpoint("productCreatedQueue", ep =>
            {
                ep.PrefetchCount = 16;
                ep.UseMessageRetry(r => r.Interval(2, 100));
                ep.ConfigureConsumer<ProductCreatedConsumer>(provider);
            });
            config.ReceiveEndpoint("productDeletedQueue", ep =>
            {
                ep.PrefetchCount = 16;
                ep.UseMessageRetry(r => r.Interval(2, 100));
                ep.ConfigureConsumer<ProductCreatedConsumer>(provider);
                ep.ConfigureConsumer<ProductDeletedConsumer>(provider);
            });
        }));
    });
    
    builder.Services.AddMassTransitHostedService();
    
    // Add services to the container.
    builder.Services.AddControllers();

    builder.Services.AddHttpClient();

    builder.Services.AddDbContext<ReservationDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

    // Add Dependency injection.
    builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    // builder.Services.AddScoped<IProductMessaging, ProductMessaging>();
    builder.Services.AddScoped<IConsumer, ProductCreatedConsumer>();
    builder.Services.AddScoped<IReservationService, ReservationService>();
    builder.Services.AddScoped<ITimeProvider, TimeProvider>();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();



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

        var context = services.GetRequiredService<ReservationDBContext>();
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
