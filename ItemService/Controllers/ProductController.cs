using ProductService.DBContexts;
using ProductService.Models;
using ProductService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using System.IO;
using System.Drawing;

namespace ProductService.Controllers
{
    /// <summary>
    /// Product controller this controller is used for the calls between API and frontend for managing the products in the ACI Rental system
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        /// <summary>
        /// Database context for the product service, this is used to make calls to the product table
        /// </summary>
        private readonly ProductServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructor is used for receiving the database context at the creation of the product controller
        /// </summary>
        /// <param name="dbContext">Database context param used for calls to the products table</param>
        public ProductController(ProductServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _dbContext.Products.ToListAsync();
        }

        /// <summary>
        /// Get latest catalog item so the front-end knows what the max is
        /// </summary>
        /// <returns>Latest catalog item</returns>
        [HttpGet("lastcatalog")]
        public async Task<ActionResult<int>> GetLastCatalog()
        {
            if(!await _dbContext.Products.AnyAsync())
            {
                return 0;
            }

            var lastCategoryItem = await _dbContext.Products.MaxAsync(x => x.CatalogNumber);
            return lastCategoryItem;
        }

        /// <summary>
        /// Endpoint to add a new product
        /// </summary>
        /// <param name="addProductModel">All the new product data</param>
        /// <returns>BadRequest if failed, Created if success</returns>
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductModel addProductModel)
        {
            if(addProductModel == default)
            {
                return BadRequest("PRODUCT.ADD.NO_DATA");
            }

            if(string.IsNullOrWhiteSpace(addProductModel.Name))
            {
                return BadRequest("PRODUCT.ADD.NO_NAME");
            }

            if (string.IsNullOrWhiteSpace(addProductModel.Description))
            {
                return BadRequest("PRODUCT.ADD.NO_NAME");
            }

            if (string.IsNullOrWhiteSpace(addProductModel.Description))
            {
                return BadRequest("PRODUCT.ADD.NO_DESCRIPTION");
            }

            if(addProductModel.CatalogNumber < 0)
            {                
                return BadRequest("PRODUCT.ADD.CATALOG_NUMBER_INCORRECT");
            }

            if(addProductModel.CategoryId < 0)
            {
                return BadRequest("PRODUCT.ADD.NO_CATEGORY");
            }

            if(await _dbContext.Products.AnyAsync(x => x.Name.Trim().ToLower() == addProductModel.Name.Trim().ToLower()))
            {
                return BadRequest("PRODUCT.ADD.NAME_ALREADY_EXISTS");
            }

            Product newProduct = new()
            {
                CatalogNumber = addProductModel.CatalogNumber,
                Name = addProductModel.Name,
                Description = addProductModel.Description,
                InventoryLocation = addProductModel.Location,
                RequiresApproval = addProductModel.RequiresApproval,
                IsAvailable = true,
                ArchivedSince = null,
                Category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == addProductModel.CategoryId)
            };

            if(newProduct.Category == default)
            {
                return BadRequest("PRODUCT.ADD.NO_CATEGORY");
            }

            if(!await MakeCatalogPlace(newProduct.CatalogNumber))
            {
                return BadRequest("PRODUCT.ADD.CATALOG_NUMBER_INCORRECT");
            }

            _dbContext.Products.Add(newProduct);
            await _dbContext.SaveChangesAsync();

            if(addProductModel.Images != default && addProductModel.Images.Any())
            {
                var addImagesObject = new AddImageModel(newProduct.Id, LinkedTableType.PRODUCT, addProductModel.Images);
                if((await $"https://localhost:44372/api/image".AllowAnyHttpStatus().PostJsonAsync(addImagesObject)).StatusCode != 201)
                {
                    _dbContext.Products.Remove(newProduct);
                    await _dbContext.SaveChangesAsync();
                    return BadRequest("PRODUCT.ADD.SAVING_IMAGES_FAILED");
                }
            }

            return Created($"/product/{newProduct.Id}", newProduct);
        }

        /// <summary>
        /// Shift all catalog items so that there are no duplicates
        /// </summary>
        /// <param name="catalogNumber">The new catalog number</param>
        /// <returns>true if success, false if failed</returns>
        private async Task<bool> MakeCatalogPlace(int catalogNumber)
        {
            if(catalogNumber < 0)
            {
                return false;
            }

            var lastCatalogNumber = 1;
            if(await _dbContext.Products.AnyAsync())
            {
                lastCatalogNumber = await _dbContext.Products.MaxAsync(x => x.CatalogNumber);
            }

            if (catalogNumber > lastCatalogNumber + 1)
            {
                return false;
            }

            if (lastCatalogNumber + 1 == catalogNumber)
            {
                return true;
            }

            var productsToChange = await _dbContext.Products.Where(x => x.CatalogNumber >= catalogNumber).ToListAsync();
            foreach (var product in productsToChange)
            {
                product.CatalogNumber++;
            }

            _dbContext.SaveChanges();

            return true;
        }

    }
}
