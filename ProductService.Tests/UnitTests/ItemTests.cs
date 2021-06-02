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

namespace ProductService.Tests.UnitTests
{
    public class ItemTests : IDisposable
    {
        private readonly ProductServiceDatabaseContext _context;
        private readonly ProductController _controller;
        private bool disposedValue;
        private readonly static object _lock = new object();
        public ItemTests()
        {
            var options = new DbContextOptionsBuilder<ProductServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryProductDb").Options;

            _context = new ProductServiceDatabaseContext(options);
            _controller = new ProductController(_context, Options.Create(new AppConfig() { ApiGatewayBaseUrl = "http://fake-url.com" }));
            SeedProductInMemoryDatabaseWithData();
        }

        [Fact]
        private async Task GetInventoryProducts_ShouldReturnFirstPageIfIndexZero()
        {
            var result = await _controller.GetInventoryItems(0, 3);

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
            var result = await _controller.GetInventoryItems(1, 2);
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
            var result = await _controller.GetInventoryItems(0, 100);
            Assert.IsType<OkObjectResult>(result);
            var actionResult = (OkObjectResult)result;

            Assert.IsType<InventoryPage>(actionResult.Value);
            var resultValue = (InventoryPage)actionResult.Value;

            Assert.Equal(9, resultValue.Products.Count());
            Assert.Equal(9, resultValue.TotalProductCount);
            Assert.Equal(0, resultValue.CurrentPage);
        }

        private async Task GetInventoryProducts_ShouldReturnLastPage()
        {
            var result = await _controller.GetInventoryItems(50, 3);
            Assert.IsType<OkObjectResult>(result);
            var actionResult = (OkObjectResult)result;

            Assert.IsType<InventoryPage>(actionResult.Value);
            var resultValue = (InventoryPage)actionResult.Value;

            Assert.Equal(3, resultValue.Products.Count());
            Assert.Equal(9, resultValue.TotalProductCount);
            Assert.Equal(3, resultValue.CurrentPage);
        }

        [Fact]
        private async Task GetInventoryProducts_ShouldReturnBadObjectResultIfPageSizeIsNegative()
        {
            var result = await _controller.GetInventoryItems(0, -3);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        private async Task GetInventoryProducts_ShouldReturnBadObjectResultIfPageIndexIsNegative()
        {
            var result = await _controller.GetInventoryItems(-3, 1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        private async Task GetInventoryProducts_ShouldReturnNotFound()
        {
            var result = await _controller.GetFlatProductById(100);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        private async Task GetFlatProductById_ShouldReturnCorrectProduct()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new ImageBlobModel
            {
                Blob = null
            });

            var result = await _controller.GetFlatProductById(7);

            Assert.IsType<OkObjectResult>(result);
            var actionResult = (OkObjectResult)result;

            Assert.IsType<ProductFlatModel>(actionResult.Value);
            var resultValue = (ProductFlatModel)actionResult.Value;

            Assert.Equal(7, resultValue.Id);
            Assert.Equal("Microphone 1", resultValue.Name);
        }

        private void SeedProductInMemoryDatabaseWithData()
        {
            lock (_lock)
            {
                var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Camera" },
                    new Category { Id = 2, Name = "Sound" },
                    new Category { Id = 3, Name = "Recording" },
                };
                if (!_context.Categories.Any())
                {
                    _context.Categories.AddRange(categories);
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
                if (!_context.Products.Any())
                {
                    _context.Products.AddRange(data);
                }
                _context.SaveChanges();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && _context != null)
                {
                    _context.Products.RemoveRange(_context.Products.ToList());
                    _context.Categories.RemoveRange(_context.Categories.ToList());
                    _context.SaveChanges();
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
