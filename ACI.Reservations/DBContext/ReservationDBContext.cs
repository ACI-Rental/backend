using ACI.Reservations.Models;
using Microsoft.EntityFrameworkCore;

namespace ACI.Reservations.DBContext
{
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Context of the database, used to communicate with the database.
    /// </summary>
    public class ReservationDBContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationDBContext"/> class.
        /// Constructor of the RaservationDBContext class.
        /// </summary>
        public ReservationDBContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationDBContext"/> class.
        /// Constructor of the ReservationServiceDatabaseContext class with options, used for Unittesting
        /// Database options can be given, to switch between local and remote database.
        /// </summary>
        /// <param name="options">Database options.</param>
        public ReservationDBContext(DbContextOptions<ReservationDBContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Sets the Reservation class, A DbSet represents the collection of all entities in the context
        /// or that can be queried from the database, of a given type. DbSet objects are created from
        /// a DbContext using the DbContext.Set method.
        /// </summary>
        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}
