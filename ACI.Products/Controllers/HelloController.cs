using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Products.Controllers;

[ApiController]
[Authorize(Roles = "employee")]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    private readonly ILogger<HelloController> _logger;

    public HelloController(ILogger<HelloController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "Hello")]
    public IActionResult GetHelloWorld()
    {
        var user = new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown",
            name = User.Identity?.Name,
        };

        return Ok($"Hello, {user}!");
    }
}