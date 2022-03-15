using ACI.Products.Data;
using ACI.Products.Domain.Product;
using ACI.Products.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductService _service;

    public ProductController(ILogger<ProductController> logger, IProductService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var result = await _service.AddProduct(dto);
        return Ok(result);
    }
}