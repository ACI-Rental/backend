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
        modelBuilder.Entity<Product>().HasIndex(p => p.CatalogPosition).IsUnique();
        modelBuilder.Entity<ProductCategory>().ToTable("ProductCategories");
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> Categories { get; set; }
    public DbSet<ProductNote> Notes { get; set; }
}