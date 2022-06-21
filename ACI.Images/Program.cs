using System;
using ACI.Images.Data;
using ACI.Images.Data.Repositories;
using ACI.Images.Data.Repositories.Interfaces;
using ACI.Images.Domain.Image;
using ACI.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting ACI.Images (version {Version})", AppUtils.GetVersion());

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
    builder.Configuration
        .AddEnvironmentVariables("ACI_");

    // Add services to the container.

    builder.Services.AddControllers();

    builder.Services.AddDbContext<ImageContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

    builder.Services.AddScoped<IImageService, ImageService>();
    builder.Services.AddScoped<IImageRepository, ImageRepository>();

    // Auth
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

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ImageContext>();
        context.Database.EnsureCreated();
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