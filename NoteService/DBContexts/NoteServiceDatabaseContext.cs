using NoteService.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace NoteService.DBContexts
{
    /// <summary>
    /// Context of the database, used to communicate with the database.
    /// </summary>
    public class NoteServiceDatabaseContext : DbContext
    {
        /// <summary>
        /// Constructor of the NoteServiceDatabaseContext class
        /// </summary>
        public NoteServiceDatabaseContext()
        {

        }

        /// <summary>
        /// Constructor of the NoteServiceDatabaseContext class with options, used for Unittesting
        /// Database options can be given, to switch between local and remote database
        /// </summary>
        /// <param name="options">Database options</param>
        public NoteServiceDatabaseContext(DbContextOptions<NoteServiceDatabaseContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet for the Note class, A DbSet represents the collection of all entities in the context. 
        /// DbSet objects are created from a DbContext using the DbContext.Set method.
        /// </summary>
        public DbSet<Note> Notes { get; set; }

        /// <summary>
        /// OnConfiguring builds the connection between the database and the API using the given connection string
        /// </summary>
        /// <param name="optionsBuilder">Used for adding options to the database to configure the connection.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbString = Environment.GetEnvironmentVariable("aci_db_string");
                if (string.IsNullOrWhiteSpace(dbString))
                {
                    throw new MissingFieldException("Database environment variable not found.");
                }

                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("aci_db_string").Replace("DATABASE_NAME", "NoteService"));
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}