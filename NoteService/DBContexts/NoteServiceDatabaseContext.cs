using NoteService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteService.DBContexts
{
    /// <summary>
    /// Context of the database, used to communicate with the database.
    /// </summary>
    public class NoteServiceDatabaseContext : DbContext
    {
        /// <summary>
        /// List of all the notes in the database.
        /// </summary>
        public DbSet<Note> Notes { get; set; }

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