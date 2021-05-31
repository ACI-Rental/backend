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

        /// <summary>
        /// Method for adding a new category that will be available to use on a product, 
        /// </summary>
        /// <param name="addCategoryModel"></param>
        /// <returns>Id of the new added category</returns>
        [HttpPost]
        public async Task<IActionResult> AddCategory(AddCategoryModel addCategoryModel)
        {
            if (addCategoryModel == default)
            {
                return BadRequest("CATEGORY.ADD.NO_DATA");
            }
            if (string.IsNullOrWhiteSpace(addCategoryModel.Name))
            {
                return BadRequest("CATEGORY.ADD.NO_NAME");
            }

            if (_dbContext.Categories.Any(o => o.Name == addCategoryModel.Name))
            {
                return BadRequest("PRODUCT.ADD.NAME_ALREADY_EXISTS");
            }

            Category newCategory = new()
            {
                Name = addCategoryModel.Name
            };

            _dbContext.Categories.Add(newCategory);
            await _dbContext.SaveChangesAsync();

            int id = newCategory.Id;

            return Ok(id);
        }

        /// <summary>
        /// Gets all categories from the database
        /// </summary>
        /// <returns>a list of categories</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var result = await _dbContext.Categories.ToListAsync();
            return Ok(result);
        }
    }
}
