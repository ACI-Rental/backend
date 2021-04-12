using Microsoft.EntityFrameworkCore;
using ReservationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationService.DBContexts
{
    /// <summary>
    /// Context of the database, used to communicate with the database.
    /// </summary>
    public class ReservationServiceDatabaseContext : DbContext
    {
        /// <summary>
        /// Constructor of the RaservationServiceDatabaseContext class
        /// </summary>
        public ReservationServiceDatabaseContext()
        {

        }

        /// <summary>
        /// Constructor of the ReservationServiceDatabaseContext class with options, used for Unittesting
        /// Database options can be given, to switch between local and remote database
        /// </summary>
        /// <param name="options">Database options</param>
        public ReservationServiceDatabaseContext(DbContextOptions<ReservationServiceDatabaseContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet for the Reservation class, A DbSet represents the collection of all entities in the context, 
        /// or that can be queried from the database, of a given type. DbSet objects are created from 
        /// a DbContext using the DbContext.Set method.
        /// </summary>
        public DbSet<Reservation> Reservations { get; set; }

        /// <summary>
        /// OnConfiguring builds the connection between the database and the API using the given connection string
        /// </summary>
        /// <param name="optionsBuilder">Used for adding options to the database to configure the connection between it and the API</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ReservationService;Trusted_Connection=True;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
