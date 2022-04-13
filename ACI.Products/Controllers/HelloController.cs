using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Products.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class HelloController : BaseController
{
    private readonly ILogger<HelloController> _logger;

    public HelloController(ILogger<HelloController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "Hello")]
    public IActionResult GetHelloWorld()
    {
        return Ok($"Hello, {GetUser()}!");
    }
}