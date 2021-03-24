using ItemService.DBContexts;
using ItemService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemService.Controllers
{
    /// <summary>
    /// Category controller this controller is used for the calls between API and frontend for managing the categories in the ACI Rental system
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        /// <summary>
        /// Database context for the category service, this is used to make calls to the category table
        /// </summary>
        public readonly ItemServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructor is used for receiving the database context at the creation of the image controller
        /// </summary>
        /// <param name="dbContext">Database context param used for calls to the category table</param>
        public CategoryController(ItemServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var result = await _dbContext.Categories.ToListAsync();
            return Ok(result);
        }
    }
}
