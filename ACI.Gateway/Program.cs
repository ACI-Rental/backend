using System;
using ACI.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting ACI.Gateway (version {Version})", AppUtils.GetVersion());

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

    builder.Host.AddAciLogging();

    var ocelotConfig = builder.Environment.IsDevelopment() ? "ocelot.Development.json" : "ocelot.json";
    builder.Configuration.AddJsonFile(ocelotConfig);



    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddOcelot();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseOcelot().Wait();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}