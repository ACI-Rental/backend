using ACI.Reservations.DBContext;
using ACI.Reservations.Models;
using ACI.Reservations.Repositories;
using ACI.Reservations.Repositories.Interfaces;
using ACI.Reservations.Services;
using ACI.Reservations.Services.Interfaces;
using ACI.Shared;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting ACI.Products microservice");
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

    builder.Services.AddDbContext<ReservationDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

    // Add Dependency injection.
    builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
    builder.Services.AddScoped<IReservationService, ReservationService>();

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

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}