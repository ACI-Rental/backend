using ProductService.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ProductService.DBContexts
{
    /// <summary>
    /// Context of the database, used to communicate with the database.
    /// </summary>
    public class ProductServiceDatabaseContext : DbContext
    {
        /// <summary>
        /// Constructor of the ProductServiceDatabaseContext class
        /// </summary>
        public ProductServiceDatabaseContext()
        {

        }

        /// <summary>
        /// Constructor of the ProductServiceDatabaseContext class with options, used for Unittesting
        /// Database options can be given, to switch between local and remote database
        /// </summary>
        /// <param name="options">Database options</param>
        public ProductServiceDatabaseContext(DbContextOptions<ProductServiceDatabaseContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet for the Products class, A DbSet represents the collection of all entities in the context, 
        /// or that can be queried from the database, of a given type. DbSet objects are created from 
        /// a DbContext using the DbContext.Set method.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// DbSet for the Category class, A DbSet represents the collection of all entities in the context. 
        /// DbSet objects are created from a DbContext using the DbContext.Set method.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// OnConfiguring builds the connection between the database and the API using the given connection string
        /// </summary>
        /// <param name="optionsBuilder">Used for adding options to the database to configure the connection between it and the API</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbString = Environment.GetEnvironmentVariable("aci_db_string");
                if (string.IsNullOrWhiteSpace(dbString))
                {
                    throw new MissingFieldException("Database environment variable not found.");
                }

                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("aci_db_string").Replace("DATABASE_NAME", "ProductService"));
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}