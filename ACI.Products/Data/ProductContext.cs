using ACI.Products.Models;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8618 // DbSets nullable and initialized by EF Core

namespace ACI.Products.Data;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory");
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> Categories { get; set; }
}