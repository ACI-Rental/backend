using ACI.Products.Data;
using ACI.Products.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly ProductContext _context;


    public CategoryController(ILogger<ProductController> logger, ProductContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDTO dto)
    {
        var category = dto.ToCategory();

        _context.Categories.Add(category);
        var result = await _context.SaveChangesAsync();

        return Ok($"{result} rows changed.");
    }
}