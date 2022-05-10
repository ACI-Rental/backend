using System;
using ACI.Products.Data;
using ACI.Products.Data.Repositories;
using ACI.Products.Data.Repositories.Interfaces;
using ACI.Products.Domain.Category;
using ACI.Products.Domain.Note;
using ACI.Products.Domain.Product;
using ACI.Products.Messaging;
using ACI.Reservations.Models;
using ACI.Shared;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
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
    builder.Services.AddScoped<INoteRepository, NoteRepository>();

    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<INoteService, NoteService>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Jwt:Authority"];
        options.Audience = builder.Configuration["Jwt:Audience"];
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
    });

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
                h.Username(builder.Configuration.GetSection(AppConfig.Key)["RabbitMQUsername"]);
                h.Password(builder.Configuration.GetSection(AppConfig.Key)["RabbitMQPassword"]);
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
        IdentityModelEventSource.ShowPII = true;
    }

    app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}

public partial class Program
{
    // Expose the Program class for use with WebApplicationFactory<T>
}