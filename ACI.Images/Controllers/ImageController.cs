using Microsoft.AspNetCore.Mvc;

namespace ACI.Images.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly ILogger<ImageController> _logger;

    public ImageController(ILogger<ImageController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Upload()
    {
        return await Task.FromResult<IActionResult>(Ok("Image uploaded!"));
    }
}