using ACI.Products.Domain.Product;
using ACI.Products.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductService _service;

    public ProductsController(ILogger<ProductsController> logger, IProductService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "employee")]
    public async Task<IActionResult> AddProduct([FromBody] CreateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        _logger.LogInformation("Adding new product {Product}", request);

        var result = await _service.AddProduct(request);

        return result
            .Right<IActionResult>(Ok)
            .Left(BadRequest);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        _logger.LogInformation("Getting product by id {ProductId}", id);
        var result = await _service.GetProductById(id);

        return result
            .Some<IActionResult>(Ok)
            .None(NotFound);
    }

    [HttpGet]
    public async Task<IActionResult> All()
    {
        _logger.LogInformation("Getting all products");
        var result = await _service.GetAllProducts();
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "employee")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        _logger.LogInformation("Deleting product by id {ProductId}", id);
        var result = await _service.DeleteProduct(id);

        return result
            .Right<IActionResult>(_ => NoContent())
            .Left(BadRequest);
    }
}