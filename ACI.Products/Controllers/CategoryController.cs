using ACI.Products.Data;
using ACI.Products.Domain.Category;
using ACI.Products.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly ICategoryService _service;

    public CategoryController(ILogger<ProductController> logger, ICategoryService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var result = await _service.CreateCategory(dto);
        return Ok(result);
    }
}