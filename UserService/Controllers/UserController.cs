using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.DBContexts;
using UserService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Controllers
{
    /// <summary>
    /// UserController this controller is used for managing the users in the ACI Rental system.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Database context for users, this is used to make calls to the database.
        /// </summary>
        private readonly UserServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructer is used for receiving the database context at the creation of the UserController.
        /// </summary>
        /// <param name="dbContext">Context of the database</param>
        public UserController(UserServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }
    }
}
