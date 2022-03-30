using System;
using System.Threading.Tasks;
using ACI.ImageService.Domain.Image;
using ACI.ImageService.Models.DTO;
using LanguageExt.UnsafeValueAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACI.ImageService.Controllers
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
        
        [HttpGet]
        public async Task<IActionResult> GetImage(Guid productId)
        {
            _logger.LogInformation("Getting Image by id {ProductId}", productId);
            
            var result = await _imageService.GetImageById(productId);
            return result
                .Some<IActionResult>(Ok)
                .None(NotFound);
        }
        
        [HttpPost]
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
    }
}