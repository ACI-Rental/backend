using System;
using System.Threading.Tasks;
using ACI.Images.Domain.Image;
using ACI.Images.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACI.Images.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> _logger;
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService, ILogger<ImageController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        [HttpGet("{productId:guid}")]
        public IActionResult GetImage(Guid productId)
        {
            _logger.LogInformation("Getting Image by id {ProductId}", productId);

            var result = _imageService.GetImageById(productId);

            return result.Right<IActionResult>(x => Ok(x)).Left(err => BadRequest(err));
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> PostImage([FromForm] UploadImageRequest uploadImageRequest)
        {
            _logger.LogInformation("Uploading image blob {ProductImageBlob}", uploadImageRequest);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _imageService.UploadImage(uploadImageRequest);

            return result.Right<IActionResult>(x => Ok(x)).Left(err => BadRequest(err));
        }

        [HttpDelete("{productId:guid}")]
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> DeleteImageById(Guid productId)
        {
            _logger.LogInformation("Deleting Image by id {ProductId}", productId);

            var result = await _imageService.DeleteImageById(productId);

            return result.Right<IActionResult>(x => NoContent()).Left(BadRequest);
        }
    }
}