using ACI.Images.Models;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8618 // DbSets nullable and initialized by EF Core

namespace ACI.Images.Data
{
    public class ImageContext : DbContext
    {
        public ImageContext(DbContextOptions<ImageContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductImageBlob>().ToTable("Image");
        }

        public DbSet<ProductImageBlob> Images { get; set; }
    }
}
