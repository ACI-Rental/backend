using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.DBContexts;
using UserService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Models.DTO;

namespace UserService.Controllers
{
    /// <summary>
    /// UserController this controller is used for managing the users in the ACI Rental system.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
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

        /// <summary>
        /// Get all the Users from the database
        /// </summary>
        /// <returns>All Users in Db</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var result = await _dbContext.Users.Include(u => u.Role).ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get all the Users for a single page
        /// </summary>
        /// <param name="pageIndex">Which page is being requested. If 0 or lower returns first page. If higher than amount of pages returns the last page </param>
        /// <param name="pageSize">The amount of users that are being requested</param>
        /// <returns>Object containing count of all users, current page and a collection of users</returns>
        [HttpGet("page/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetUsersPage(int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                return BadRequest("USERS.INCORRECT_INDEX");
            }

            if (pageSize < 0)
            {
                return BadRequest("USERS.INCORRECT_INDEX");
            }

            var page = new UsersPage();

            var query = from user in _dbContext.Users
                        orderby user.StudentNumber ascending
                        select new User()
                        {
                            StudentNumber = user.StudentNumber,
                            BannedUntil = user.BannedUntil,
                            Role = user.Role
                        };

            page.TotalUserCount = await query.CountAsync();

            // Last page calculation goes wrong if the totalcount is 0
            // also no point in trying to get 0 products from DB
            if (page.TotalUserCount == 0)
            {
                page.CurrentPage = 0;
                page.Users = new List<User>(0);
                return Ok(page);
            }

            // calculate how many pages there are given de current pageSize
            int lastPage = (int)Math.Ceiling((double)page.TotalUserCount / pageSize) - 1;

            // pageIndex below 0 is nonsensical, bringing the value to closest sane value
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }

            // use last page if requested page is higher
            page.CurrentPage = Math.Min(pageIndex, lastPage);

            page.Users = await (query).Skip(page.CurrentPage * pageSize).Take(pageSize).ToListAsync();
            return Ok(page);
        }

    }
}
