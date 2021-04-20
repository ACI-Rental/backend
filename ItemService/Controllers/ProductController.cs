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

        /// <summary>
        /// Get all the users from the database
        /// </summary>
        /// <returns>All Users in Db</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var result = await _dbContext.Products.ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get the product with a certain id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>> GetProduct(int productId)
        {
            return await _dbContext.Products.Where(x => x.Id == productId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get a single inventory page containing products
        /// </summary>
        /// <param name="pageIndex">Which page is being requested. If 0 or lower returns first page. If higher than amount of pages returns the last page </param>
        /// <param name="pageSize">The amount of products that are being requested</param>
        /// <returns>Object containing count of all products, current page and a collection of products</returns>
        [HttpGet("page/{pageIndex}/{pageSize}")]
        public async Task<InventoryPage> GetInventoryItems(int pageIndex, int pageSize)
        {
            var page = new InventoryPage();

            var query = from product in _dbContext.Products
                        orderby product.CatalogNumber ascending
                        select new InventoryProduct()
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Location = product.InventoryLocation,
                            RequiresApproval = product.RequiresApproval,
                            Status = product.ProductState
                        };

            page.TotalProductCount = await query.CountAsync();

            // Last page calculation goes wrong if the totalcount is 0
            // also no point in trying to get 0 products from DB
            if (page.TotalProductCount == 0)
            {
                page.CurrentPage = 0;
                page.Products = new List<InventoryProduct>(0);
                return page;
            }

            // calculate how many pages there are given de current pageSize
            int lastPage = (int)Math.Ceiling((double)page.TotalProductCount / pageSize) - 1;

            // pageIndex below 0 is non-sensical, bringing the value to closest sane value
            if (pageIndex < 0)
                pageIndex = 0;

            // use lastpage if requested page is higher
            page.CurrentPage = Math.Min(pageIndex, lastPage);

            page.Products = await (query).Skip(page.CurrentPage * pageSize).Take(pageSize).ToListAsync();
            return page;
        }

        /// <summary>
        /// Used to receive a very basic/stripped product class with minimal data based on productId
        /// </summary>
        /// <param name="productId">Id of the product to look for</param>
        /// <returns>Found (and stripped) product</returns>
        [HttpGet("flat/{productId}")]
        public async Task<IActionResult> GetFlatProductById(int productId)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);

            if (product == default)
            {
                return NotFound("API_RESPONSES.PRODUCT.GET_PRODUCT_BY_ID.NOT_FOUND");
            }

            var cartProduct = new ProductFlatModel()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ProductState = product.ProductState
            };

            var image = await $"https://localhost:44372/api/image/{product.Id}".AllowAnyHttpStatus().GetJsonAsync<ImageBlobModel>();
            if(image != default && image.Blob != default)
            {
                cartProduct.Image = Convert.ToBase64String(image.Blob);
            }

            return Ok(cartProduct);
        }

        /// <summary>
        /// Get latest catalog item so the front-end knows what the max is
        /// </summary>
        /// <returns>Latest catalog item</returns>
        [HttpGet("lastcatalog")]
        public async Task<ActionResult<int>> GetLastCatalog()
        {
            if (!await _dbContext.Products.AnyAsync())
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
            if (addProductModel == default)
            {
                return BadRequest("PRODUCT.ADD.NO_DATA");
            }

            if (string.IsNullOrWhiteSpace(addProductModel.Name))
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

            if (addProductModel.CatalogNumber < 0)
            {
                return BadRequest("PRODUCT.ADD.CATALOG_NUMBER_INCORRECT");
            }

            if (addProductModel.CategoryId < 0)
            {
                return BadRequest("PRODUCT.ADD.NO_CATEGORY");
            }

            if (await _dbContext.Products.AnyAsync(x => x.Name.Trim().ToLower() == addProductModel.Name.Trim().ToLower()))
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
                ProductState = ProductState.AVAILABLE,
                Category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == addProductModel.CategoryId)
            };

            if (newProduct.Category == default)
            {
                return BadRequest("PRODUCT.ADD.NO_CATEGORY");
            }

            if (!await MakeCatalogPlace(newProduct.CatalogNumber))
            {
                return BadRequest("PRODUCT.ADD.CATALOG_NUMBER_INCORRECT");
            }

            _dbContext.Products.Add(newProduct);
            await _dbContext.SaveChangesAsync();

            if (addProductModel.Images != default && addProductModel.Images.Any())
            {
                var addImagesObject = new AddImageModel(newProduct.Id, LinkedTableType.PRODUCT, addProductModel.Images);
                if ((await $"https://localhost:44372/api/image".AllowAnyHttpStatus().PostJsonAsync(addImagesObject)).StatusCode != 201)
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
            if (catalogNumber < 0)
            {
                return false;
            }

            var lastCatalogNumber = 1;
            if (await _dbContext.Products.AnyAsync())
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

        /// <summary>
        /// Archives a product so it can't be used anymore
        /// </summary>
        /// <param name="archiveProductId">The id for the product that needs to be archived</param>
        /// <returns>BadRequest if data is incorrect, success message if successful</returns>
        [HttpDelete("{archiveProductId}")]
        public async Task<IActionResult> ArchiveProduct(int archiveProductId)
        {
            if (archiveProductId <= 0)
            {
                return BadRequest("PRODUCT.ARCHIVE.NO_VALID_ID");
            }
            var foundProduct = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == archiveProductId);
            if (foundProduct == null)
            {
                return BadRequest("PRODUCT.ARCHIVE.NO_PRODUCT_FOUND");
            }
            if (foundProduct.ProductState == ProductState.ARCHIVED)
            {
                return BadRequest("PRODUCT.ARCHIVE.PRODUCT_ALREADY_ARCHIVED");
            }
            //TODO: Sends mail to all persons that have already rented this product.
            foundProduct.ProductState = ProductState.ARCHIVED;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
