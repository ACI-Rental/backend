using System;
using System.Collections.Generic;
using Xunit;
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
    public class ArchiveTests : IDisposable
    {
        private readonly ProductController _controller;
        private readonly ProductServiceDatabaseContext _context;

        public ArchiveTests() 
        {
            var options = new DbContextOptionsBuilder<ProductServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryReservationDb").Options;

            _context = new ProductServiceDatabaseContext(options);
            _controller = new ProductController(_context, Options.Create(new AppConfig() { ApiGatewayBaseUrl = "http://fake-url.com" }));

            SeedProductsInMemoryDatabase();
        }
        
        [Fact]
        public async Task ArchiveProduct_ShouldReturnNoValidIdError()
        {
            IActionResult result = await _controller.ArchiveProduct(0);
            var actionResult = result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
            Assert.Equal("PRODUCT.ARCHIVE.NO_VALID_ID", actionResult.Value);
        }

        [Fact]
        public async Task ArchiveProduct_ShouldReturnNoProductFoundError()
        {
            IActionResult result = await _controller.ArchiveProduct(3);
            var actionResult = result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
            Assert.Equal("PRODUCT.ARCHIVE.NO_PRODUCT_FOUND", actionResult.Value);
        }

        [Fact]
        public async Task ArchiveProduct_ShouldReturnAlreadyArchivedError()
        {
            IActionResult result = await _controller.ArchiveProduct(2);
            var actionResult = result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
            Assert.Equal("PRODUCT.ARCHIVE.PRODUCT_ALREADY_ARCHIVED", actionResult.Value);
        }

        [Fact]
        public async Task ArchiveProduct_ShouldReturnOkForSuccessfulArchive()
        {
            var foundProduct = _context.Products.Where(x => x.Id == 1).FirstOrDefault();
            Assert.Equal(Models.DTO.ProductState.AVAILABLE, foundProduct.ProductState);
            IActionResult result = await _controller.ArchiveProduct(1);
            var actionResult = result as OkResult;
            foundProduct = _context.Products.Where(x => x.Id == 1).FirstOrDefault();
            Assert.IsType<OkResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
            Assert.Equal(Models.DTO.ProductState.ARCHIVED, foundProduct.ProductState);
        }

        /// <summary>
        /// Sets the data in the database for every test
        /// </summary>
        private void SeedProductsInMemoryDatabase()
        {
            var category = new Category() { Id = 1, Name = "Camera" };
            var data = new List<Product>
            {
                new Product() 
                {
                    Id = 1,
                    Name = "Video Camera 1",
                    CatalogNumber = 1,
                    Category = category,
                    InventoryLocation = "Shelf 4",
                    ProductState = Models.DTO.ProductState.AVAILABLE,
                    RequiresApproval = false,
                    Description = "New videocamera"
                },
                new Product()
                {
                    Id = 2,
                    Name = "Video Camera 2",
                    CatalogNumber = 2,
                    Category = category,
                    InventoryLocation = "Shelf 5",
                    ProductState = Models.DTO.ProductState.ARCHIVED,
                    RequiresApproval = false,
                    Description = "New videocamera"
                }
            };
            _context.Products.AddRange(data);
            _context.SaveChanges();
        }

        /// <summary>
        /// Disposes handles cleaning of the database entries after each test
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _context != null)
            {
                _context.Products.RemoveRange(_context.Products.ToList());
                _context.Categories.RemoveRange(_context.Categories.ToList());
                _context.SaveChanges();
                _context.Dispose();
            }
        }
    }
}
