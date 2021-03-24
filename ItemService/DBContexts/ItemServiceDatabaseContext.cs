using ItemService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemService.DBContexts
{
    public class ItemServiceDatabaseContext : DbContext
    {
        /// <summary>
        /// DbSet for the Items class, A DbSet represents the collection of all entities in the context, 
        /// or that can be queried from the database, of a given type. DbSet objects are created from 
        /// a DbContext using the DbContext.Set method.
        /// </summary>
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        /// <summary>
        /// OnConfiguring builds the connection between the database and the API using the given connection string
        /// </summary>
        /// <param name="optionsBuilder">Used for adding options to the database to configure the connection between it and the API</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ItemService;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
