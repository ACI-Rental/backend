using ItemService.Controllers;
using ItemService.DBContexts;
using ItemService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ItemService.Tests.UnitTests
{
    public class ItemTests
    {
        [Fact]
        public async Task GetProducts_WhenCalled_ReturnListOfProducts()
        {
            var options = new DbContextOptionsBuilder<ItemServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryProductDb").Options;

            var context = new ItemServiceDatabaseContext(options);
            SeedProductInMemoryDatabaseWithData(context);
            var controller = new ItemController(context);
            var result = await controller.GetItems();

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<Item>>(objectresult.Value);

            Assert.Equal(3, products.Count());
            Assert.Equal("BBB", products.ElementAt(0).Name);
            Assert.Equal("ZZZ", products.ElementAt(1).Name);
            Assert.Equal("AAA", products.ElementAt(2).Name);
        }

        private static void SeedProductInMemoryDatabaseWithData(ItemServiceDatabaseContext context)
        {
            var data = new List<Item>
                {
                    new Item { Name = "BBB" },
                    new Item { Name = "ZZZ" },
                    new Item { Name = "AAA" },
                };
            context.Items.AddRange(data);
            context.SaveChanges();

        }
    }
}
