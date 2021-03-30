using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteService.DBContexts;
using NoteService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteService.Controllers
{
    /// <summary>
    /// NoteController this controller is used for managing the notes in the ACI Rental system.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class NoteController : ControllerBase
    {
        /// <summary>
        /// Database context for notes, this is used to make calls to the database.
        /// </summary>
        private readonly NoteServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructer is used for receiving the database context at the creation of the NoteController.
        /// </summary>
        /// <param name="dbContext">Context of the database</param>
        public NoteController(NoteServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            return await _dbContext.Notes.ToListAsync();
        }
    }
}
