using ItemService.DBContexts;
using ItemService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemService.Controllers
{
    /// <summary>
    /// Item controller this controller is used for the calls between API and frontend for managing the items in the ACI Rental system
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        /// <summary>
        /// Database context for the item service, this is used to make calls to the item table
        /// </summary>
        public readonly ItemServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructor is used for receiving the database context at the creation of the item controller
        /// </summary>
        /// <param name="dbContext">Database context param used for calls to the item table</param>
        public ItemController(ItemServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Camera canon", "Camera sony" };
        }

        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "Camera1";
        }
    }
}
