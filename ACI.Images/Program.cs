using ACI.ImageService.Data;
using ACI.ImageService.Data.Repositories;
using ACI.ImageService.Data.Repositories.Interfaces;
using ACI.ImageService.Domain.Image;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ImageContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();


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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();