using System;
using System.Collections.Generic;
using Xunit;
using ItemService;
using ItemService.Controllers;
using ItemService.Models;
using ItemService.DBContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ItemService.Tests.UnitTests
{
    public class CategoryTests
    {
        [Fact]
        public async Task GetCategories_WhenCalled_ReturnListOfCategories()
        {
            var options = new DbContextOptionsBuilder<ItemServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryProductDb").Options;

            var context = new ItemServiceDatabaseContext(options);
            SeedCategoryInMemoryDatabaseWithData(context);
            var controller = new CategoryController(context);
            var result = await controller.GetCategories();

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var categories = Assert.IsAssignableFrom<IEnumerable<Category>>(objectresult.Value);

            Assert.Equal(3, categories.Count());
            Assert.Equal("BBB", categories.ElementAt(0).Name);
            Assert.Equal("ZZZ", categories.ElementAt(1).Name);
            Assert.Equal("AAA", categories.ElementAt(2).Name);
        }

        private static void SeedCategoryInMemoryDatabaseWithData(ItemServiceDatabaseContext context)
        {
            var data = new List<Category>
                {
                    new Category { Name = "BBB" },
                    new Category { Name = "ZZZ" },
                    new Category { Name = "AAA" },
                };
            context.Categories.AddRange(data);
            context.SaveChanges();
            
        }
    }
}
