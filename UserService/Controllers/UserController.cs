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
        /// Constructor is used for receiving the database context at the creation of the UserController.
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
                        orderby user.Id ascending
                        select new OverviewUser()
                        {
                            StudentNumber = user.UserInfo.Studentnumber,
                            Name = user.UserInfo.Name,
                            BannedUntil = user.BannedUntil,
                            Role = user.Role
                        };

            page.TotalUsersCount = await query.CountAsync();

            // Last page calculation goes wrong if the totalcount is 0
            // also no point in trying to get 0 products from DB
            if (page.TotalUsersCount == 0)
            {
                page.CurrentPage = 0;
                page.Users = new List<OverviewUser>(0);
                return Ok(page);
            }

            // calculate how many pages there are given de current pageSize
            int lastPage = (int)Math.Ceiling((double)page.TotalUsersCount / pageSize) - 1;

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

        /// <summary>
        /// An action that can be used on a user, updating the database
        /// Actions that can be done are: Block and Unblock
        /// </summary>
        /// <param name="userBlockActionModel">Action Object with User Id, Action number and Enddate for the block</param>
        /// <returns>Ok</returns>
        [HttpPost("blockaction")]
        public async Task<IActionResult> BlockActionCall(UserBlockActionModel userBlockActionModel)
        {
            if (userBlockActionModel == null)
            {
                return BadRequest("USERS.ACTION.INVALID.CALL");
            }

            if (userBlockActionModel.userId < 0)
            {
                return BadRequest("USERS.ACTION.INVALID.ID");
            }

            if ( userBlockActionModel.Action == UserBlockAction.BLOCK && userBlockActionModel.blockUntil < DateTime.Now)
            {
                return BadRequest("USERS.ACTION.INVALID.ACTION");
            }

            var user = _dbContext.Users.Include(u => u.Role).SingleOrDefault(x => x.Id == userBlockActionModel.userId);
            if (user != null)
            {
                if (user.Role.Name != "Admin")
                {
                    switch (userBlockActionModel.Action)
                    {
                        case UserBlockAction.BLOCK:
                            user.BannedUntil = userBlockActionModel.blockUntil;
                            break;
                        case UserBlockAction.UNBLOCK:
                            user.BannedUntil = null;
                            break;
                        default:
                            return BadRequest("USERS.ACTION.INVALID.ACTION");
                    }
                }
                else
                {
                    return BadRequest("USERS.ACTION.INVALID.BLOCK_ADMIN");
                }
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }



    }
}
