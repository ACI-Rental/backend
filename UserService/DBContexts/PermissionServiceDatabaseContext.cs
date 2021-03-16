using Microsoft.EntityFrameworkCore;
using UserService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.DBContexts
{
    /// <summary>
    /// Context of the database, used to communicate with the database.
    /// </summary>
    public class PermissionServiceDatabaseContext : DbContext
    {
        /// <summary>
        /// DbSet for the Permission class, A DbSet represents the collection of all entities in the context, 
        /// or that can be queried from the database, of a given type. DbSet objects are created from 
        /// a DbContext using the DbContext.Set method.
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Creates a connection with the database.
        /// </summary>
        /// <param name="optionsBuilder">ContextBuilder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=mssql.fhict.local;Database=dbi331842;User Id=dbi331842;Password=xRqMZgWy76GrxM2;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}