using System;
using System.Threading.Tasks;
using ACI.Products.Domain.Product;
using ACI.Products.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACI.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : BaseController
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

    [HttpPut("Archive")]
    [Authorize(Roles = "employee")]
    public async Task<IActionResult> ArchiveProduct([FromBody] ProductArchiveRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        _logger.LogInformation("Archiving product {Product}", request);

        var result = await _service.ArchiveProduct(request);

        return result
            .Right<IActionResult>(Ok)
            .Left(BadRequest);
    }

    [HttpPut("Edit")]
    [Authorize(Roles = "employee")]
    public async Task<IActionResult> EditProduct([FromBody] ProductUpdateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        _logger.LogInformation("Editing product {Product}", request);

        var result = await _service.EditProduct(request);

        return result
            .Right<IActionResult>(Ok)
            .Left(BadRequest);
    }
}