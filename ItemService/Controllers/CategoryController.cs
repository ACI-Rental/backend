using ProductService.DBContexts;
using ProductService.Models;
using ProductService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Controllers
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
        public readonly ProductServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructor is used for receiving the database context at the creation of the category controller
        /// </summary>
        /// <param name="dbContext">Database context param used for calls to the category table</param>
        public CategoryController(ProductServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _dbContext.Categories.ToListAsync();
        }
    }
}
