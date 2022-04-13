using ACI.Products.Domain.Category;
using ACI.Products.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : BaseController
{
    private readonly ILogger<ProductsController> _logger;
    private readonly ICategoryService _service;

    public CategoryController(ILogger<ProductsController> logger, ICategoryService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "employee")]
    public async Task<IActionResult> AddCategory([FromBody] CreateCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        _logger.LogInformation("Adding new category {Category}", request);

        var result = await _service.CreateCategory(request);

        return result
            .Right<IActionResult>(Ok)
            .Left(BadRequest);
    }

    [HttpGet]
    public async Task<IActionResult> All()
    {
        _logger.LogInformation("Getting all categories");
        var result = await _service.GetCategories();
        return Ok(result);
    }
}