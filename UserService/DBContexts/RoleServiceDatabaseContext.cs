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
    public class RoleServiceDatabaseContext : DbContext
    {
        /// <summary>
        /// List of all the roles in the database.
        /// </summary>
        public DbSet<Role> Roles { get; set; }

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