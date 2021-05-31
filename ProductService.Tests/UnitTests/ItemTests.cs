using System;
using System.Collections.Generic;
using Xunit;
using ProductService;
using ProductService.Controllers;
using ProductService.Models;
using ProductService.DBContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ProductService.Tests.UnitTests
{
    public class ItemTests
    {
        [Fact]
        public async Task GetProducts_WhenCalled_ReturnListOfProducts()
        {
            var options = new DbContextOptionsBuilder<ProductServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryProductDb").Options;

            var context = new ProductServiceDatabaseContext(options);
            SeedProductInMemoryDatabaseWithData(context);
            var controller = new ProductController(context, Options.Create(new AppConfig() { ApiGatewayBaseUrl = "http://fake-url.com" }));
            var result = await controller.GetProducts();

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(objectresult.Value);

            Assert.Equal(3, products.Count());
            Assert.Equal("BBB", products.ElementAt(0).Name);
            Assert.Equal("ZZZ", products.ElementAt(1).Name);
            Assert.Equal("AAA", products.ElementAt(2).Name);
        }

        private static void SeedProductInMemoryDatabaseWithData(ProductServiceDatabaseContext context)
        {
            var data = new List<Product>
                {
                    new Product { Name = "BBB" },
                    new Product { Name = "ZZZ" },
                    new Product { Name = "AAA" },
                };
            context.Products.AddRange(data);
            context.SaveChanges();

        }
    }
}
