using ImageService.DBContexts;
using ImageService.Models;
using ImageService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageService.Controllers
{
    /// <summary>
    /// Image controller this controller is used for the calls between API and frontend for managing the images in the ACI Rental system
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// Database context for the image service, this is used to make calls to the image table
        /// </summary>
        private readonly ImageServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructor is used for receiving the database context at the creation of the image controller
        /// </summary>
        /// <param name="dbContext">Database context param used for calls to the image table</param>
        public ImageController(ImageServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds image to the database
        /// </summary>
        /// <param name="addImageModel">The API call data object</param>
        /// <returns>Badrequest if fails, Created if success </returns>
        [HttpPost]
        public async Task<IActionResult> SaveImages(AddImageModel addImageModel)
        {
            if(addImageModel == default)
            {
                return BadRequest("NO_DATA");
            }

            if (addImageModel.Base64Images == default || !addImageModel.Base64Images.Any())
            {
                return BadRequest("NO_IMAGES");
            }

            if(addImageModel.LinkedPrimaryKey < 1)
            {
                return BadRequest("NO_LINKED_KEY");
            }

            List<Image> images = new List<Image>();

            foreach (var image in addImageModel.Base64Images)
            {
                if (CheckImage(new CheckImageModel() { Base64Image = image }).GetType() != typeof(OkResult))
                {
                    return BadRequest("File is not an image");
                }

                var newImage = new Image()
                {
                    LinkedKey = addImageModel.LinkedPrimaryKey,
                    LinkedTableType = addImageModel.LinkedTableType,
                    Blob = Convert.FromBase64String(image[(image.IndexOf(",") + 1)..])
                };

                images.Add(newImage);
            }

            await _dbContext.Images.AddRangeAsync(images);
            await _dbContext.SaveChangesAsync();

            return Created("/image", images);
        }

        /// <summary>
        /// Checks if Base64 string is an image
        /// </summary>
        /// <param name="checkImageModel">Object containing the Base64 string</param>
        /// <returns>Badrequest if it's not an image. Ok if it is an image</returns>
        [HttpPost("checkimage")]
        public IActionResult CheckImage(CheckImageModel checkImageModel)
        {
            if(checkImageModel == default || string.IsNullOrWhiteSpace(checkImageModel.Base64Image))
            {
                return BadRequest("Request did not contain data");
            }


            if(!checkImageModel.Base64Image.Contains(","))
            {
                return BadRequest("Base64string is not JS based Base64 (does not contain type)");
            }

            var acceptedImageTypes = new string[] { "png", "jpeg" };

            var fileType = checkImageModel.Base64Image.Split(",")[0];
            if (!acceptedImageTypes.Any(fileType.Contains))
            {
                return BadRequest("Incorrect image type");
            }

            try
            {
                using var ms = new MemoryStream(Convert.FromBase64String(checkImageModel.Base64Image[(checkImageModel.Base64Image.IndexOf(",") + 1)..]));
                var img = System.Drawing.Image.FromStream(ms);
                img.Dispose();
            }
            catch (Exception)
            {
                return BadRequest("Incorrect image type");
            }

            return Ok();
        }
    }
}
