using ACI.Products.Data;
using ACI.Products.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly ProductContext _context;

    public ProductController(ILogger<ProductController> logger, ProductContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] CreateProductDTO dto)
    {
        var product = dto.ToProduct();

        _context.Products.Add(product);

        var result = await _context.SaveChangesAsync();

        return Ok($"{result} rows changed.");
    }
}