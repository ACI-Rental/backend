using ImageService.DBContexts;
using ImageService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        public readonly ImageServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructor is used for receiving the database context at the creation of the image controller
        /// </summary>
        /// <param name="dbContext">Database context param used for calls to the image table</param>
        public ImageController(ImageServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "image", "image2" };
        }

        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "image3";
        }
    }
}
