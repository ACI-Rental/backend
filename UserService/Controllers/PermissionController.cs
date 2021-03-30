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
    /// PermissionController this controller is used for managing the permissions in the ACI Rental system.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        /// <summary>
        /// Database context for Permissions, this is used to make calls to the database.
        /// </summary>
        private readonly UserServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructer is used for receiving the database context at the creation of the PermissionController.
        /// </summary>
        /// <param name="dbContext">Context of the database</param>
        public PermissionController(UserServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permission>>> GetPermissions()
        {
            return await _dbContext.Permissions.ToListAsync();
        }
    }
}
