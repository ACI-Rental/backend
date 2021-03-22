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
    public class UserServiceDatabaseContext : DbContext
    {
        /// <summary>
        /// List of all the users in the database.
        /// </summary>
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Creates a connection with the database.
        /// </summary>
        /// <param name="optionsBuilder">ContextBuilder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=UserService;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}