using System;
using System.Collections.Generic;
using Xunit;
using ProductService;
using ProductService.Controllers;
using ProductService.Models;
using ProductService.Models.DTO;
using ProductService.DBContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Flurl.Http.Testing;
using System.Runtime.CompilerServices;

namespace ProductService.Tests.UnitTests
{
    public class ProductTests
    { 

        /// <summary>
        /// Initializes the test by creating the database and controller
        /// </summary>
        /// <param name="seed">Whether the database should be fileld with data</param>
        /// <param name="callerName">The name of the method. 
        /// Gets filled using the CallerMemberName attribute
        /// </param>
        /// <returns>The product controller set up for testing</returns>
        private ProductController Initialize(bool seed = true, [CallerMemberName]string callerName = "")
        {
            var options = new DbContextOptionsBuilder<ProductServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryProductDb_" + callerName).Options;
            var context = new ProductServiceDatabaseContext(options);
            if (seed)
            {
                SeedProductInMemoryDatabaseWithData(context);
            }

            return new ProductController(context, Options.Create(new AppConfig() { ApiGatewayBaseUrl = "http://fake-url.com" }));
        }

        [Fact]
        private async Task GetInventoryProducts_ShouldReturnFirstPageIfIndexZero()
        {
            var controller = Initialize();
            var result = await controller.GetInventoryItems(0, 3);

            Assert.IsType<OkObjectResult>(result);
            var actionResult = (OkObjectResult)result;

            Assert.IsType<InventoryPage>(actionResult.Value);
            var resultValue = (InventoryPage)actionResult.Value;

            Assert.Equal(3, resultValue.Products.Count());
            Assert.Equal(0, resultValue.CurrentPage);
            Assert.Equal(1, resultValue.Products.First().Id);

        }

        [Fact]
        private async Task GetInventoryProducts_ShouldReturnSecondPageIfIndexOne()
        {
            var controller = Initialize();
            var result = await controller.GetInventoryItems(1, 2);
            Assert.IsType<OkObjectResult>(result);
            var actionResult = (OkObjectResult)result;

            Assert.IsType<InventoryPage>(actionResult.Value);
            var resultValue = (InventoryPage)actionResult.Value;

            Assert.Equal(2, resultValue.Products.Count());
            Assert.Equal(1, resultValue.CurrentPage);

            var firstProduct = resultValue.Products.First();
            Assert.Equal(3, firstProduct.Id);
            Assert.Equal("XA 15 prof Camcorder", firstProduct.Name);
        }

        [Fact]
        private async Task GetInventoryProducts_ShouldReturnAllProducts()
        {
            var controller = Initialize();
            var result = await controller.GetInventoryItems(0, 100);
            Assert.IsType<OkObjectResult>(result);
            var actionResult = (OkObjectResult)result;

            Assert.IsType<InventoryPage>(actionResult.Value);
            var resultValue = (InventoryPage)actionResult.Value;

            Assert.Equal(9, resultValue.Products.Count());
            Assert.Equal(9, resultValue.TotalProductCount);
            Assert.Equal(0, resultValue.CurrentPage);
        }

        [Fact]
        private async Task GetInventoryProducts_ShouldReturnLastPage()
        {
            var controller = Initialize();
            var result = await controller.GetInventoryItems(50, 3);
            Assert.IsType<OkObjectResult>(result);
            var actionResult = (OkObjectResult)result;

            Assert.IsType<InventoryPage>(actionResult.Value);
            var resultValue = (InventoryPage)actionResult.Value;

            Assert.Equal(3, resultValue.Products.Count());
            Assert.Equal(9, resultValue.TotalProductCount);
            Assert.Equal(2, resultValue.CurrentPage);
        }

        [Fact]
        private async Task GetInventoryProducts_ShouldReturnBadObjectResultIfPageSizeIsNegative()
        {
            var controller = Initialize(seed: false);
            var result = await controller.GetInventoryItems(0, -3);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        private async Task GetInventoryProducts_ShouldReturnBadObjectResultIfPageIndexIsNegative()
        {
            var controller = Initialize(seed: false);
            var result = await controller.GetInventoryItems(-3, 1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        private async Task GetLastCatalog_ShouldReturn0IfCatalogEmpty()
        {
            var controller = Initialize(seed: false);
            var result = await controller.GetLastCatalog();

            var resultValue = result.Value;

            Assert.Equal(0, resultValue);
        }

        [Fact]
        private async Task GetLastCatalog_ShouldReturnLastCatalogNumber()
        {
            var controller = Initialize();
            var result = await controller.GetLastCatalog();

            var resultValue = result.Value;

            Assert.Equal(9, resultValue);
        }

        [Fact]
        private async Task GetFlatProductById_ShouldReturnNotFoundError()
        {
            var controller = Initialize();
            var result = await controller.GetFlatProductById(100);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        private async Task GetFlatProductById_ShouldReturnCorrectProduct()
        {
            var controller = Initialize();
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new ImageBlobModel
            {
                Blob = null
            });

            var result = await controller.GetFlatProductById(7);

            Assert.IsType<OkObjectResult>(result);
            var actionResult = (OkObjectResult)result;

            Assert.IsType<ProductFlatModel>(actionResult.Value);
            var resultValue = (ProductFlatModel)actionResult.Value;

            Assert.Equal(7, resultValue.Id);
            Assert.Equal("Microphone 1", resultValue.Name);
        }


        [Fact]
        private async Task GetInventoryProducts_ShouldReturnNotFound()
        {
            var controller = Initialize(seed: false);
            var result = await controller.GetFlatProductById(100);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        private async Task AddProduct_ShouldCreateObject()
        {
            var controller = Initialize();
            using var httpTest = new HttpTest();
            var productModel = new AddProductModel()
            {
                Name = "NewProduct",
                CatalogNumber = 2,
                Location = "Plek 3",
                RequiresApproval = false,
                CategoryId = 1,
                Description = "Fusce consectetur luctus urna. Vestibulum feugiat id sem vitae placerat."
            };
            var result = await controller.AddProduct(productModel);

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        private async Task AddProduct_ShouldReturnBadRequestIfParameterNull()
        {
            var controller = Initialize();
            using var httpTest = new HttpTest();
            var result = await controller.AddProduct(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        private async Task AddProduct_ShouldReturnBadRequestWithoutCategory()
        {
            var controller = Initialize();
            using var httpTest = new HttpTest();
            var productModel = new AddProductModel()
            {
                Name = "NewProduct",
                CatalogNumber = 2,
                Location = "Plek 3",
                RequiresApproval = false,
                Description = "Fusce consectetur luctus urna. Vestibulum feugiat id sem vitae placerat."
            };
            var result = await controller.AddProduct(productModel);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        private async Task AddProduct_ShouldReturnBadRequestWithoutName()
        {
            var controller = Initialize();
            using var httpTest = new HttpTest();
            var productModel = new AddProductModel()
            {
                CatalogNumber = 2,
                Location = "Plek 3",
                RequiresApproval = false,
                CategoryId = 1,
                Description = "Fusce consectetur luctus urna. Vestibulum feugiat id sem vitae placerat."
            };
            var result = await controller.AddProduct(productModel);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        private async Task AddProduct_ShouldReturnBadRequestIfCatalogNegative()
        {
            var controller = Initialize();
            using var httpTest = new HttpTest();
            var productModel = new AddProductModel()
            {
                Name = "NewProduct",
                CatalogNumber = -2,
                Location = "Plek 3",
                RequiresApproval = false,
                CategoryId = 1,
                Description = "Fusce consectetur luctus urna. Vestibulum feugiat id sem vitae placerat."
            };
            var result = await controller.AddProduct(productModel);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        private void SeedProductInMemoryDatabaseWithData(ProductServiceDatabaseContext context)
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Camera" },
                new Category { Id = 2, Name = "Sound" },
                new Category { Id = 3, Name = "Recording" },
            };
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(categories);
            }

            var data = new List<Product>
            {
               new Product { Id = 1, Name = "Canon EOS R5", CatalogNumber = 1, Category = categories[0], Description = "Dit is tekst", InventoryLocation="Balie", ProductState = ProductState.AVAILABLE, RequiresApproval = false},
               new Product { Id = 2, Name = "Canon 80D", CatalogNumber = 2, Category = categories[0], Description = "Dit is tekst", InventoryLocation="Balie", ProductState = ProductState.UNAVAILABLE, RequiresApproval = false},
               new Product { Id = 3, Name = "XA 15 prof Camcorder", CatalogNumber = 3, Category = categories[0], Description = "Dit is tekst", InventoryLocation="Balie", ProductState = ProductState.ARCHIVED, RequiresApproval = false},

               new Product { Id = 4, Name = "DJ Set", CatalogNumber = 6, Category = categories[1], Description = "Dit is tekst", InventoryLocation="A3.5", ProductState = ProductState.AVAILABLE, RequiresApproval = true},
               new Product { Id = 5, Name = "Speakers", CatalogNumber = 4, Category = categories[1], Description = "Dit is tekst", InventoryLocation="Plank A2", ProductState = ProductState.UNAVAILABLE, RequiresApproval = true},
               new Product { Id = 6, Name = "Headset", CatalogNumber = 5, Category = categories[1], Description = "Dit is tekst", InventoryLocation="Plank 1", ProductState = ProductState.ARCHIVED, RequiresApproval = true},

               new Product { Id = 7, Name = "Microphone 1", CatalogNumber = 9, Category = categories[2], Description = "Dit is tekst", InventoryLocation="A3.5", ProductState = ProductState.AVAILABLE, RequiresApproval = false},
               new Product { Id = 8, Name = "Microphone 2", CatalogNumber = 7, Category = categories[2], Description = "Dit is tekst", InventoryLocation="Plank A2", ProductState = ProductState.UNAVAILABLE, RequiresApproval = true},
               new Product { Id = 9, Name = "Microphone 3", CatalogNumber = 8, Category = categories[2], Description = "Dit is tekst", InventoryLocation="Plank 1", ProductState = ProductState.ARCHIVED, RequiresApproval = false},
            };
            if (!context.Products.Any())
            {
                context.Products.AddRange(data);
            }
            context.SaveChanges();

        }
    }
}
