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
using ProductService.Models.DTO;
using System.Net;

namespace ProductService.Tests.UnitTests
{
    public class CategoryTests : IDisposable
    {
        private CategoryController _controller;
        private ProductServiceDatabaseContext _context;

        public CategoryTests()
        {
            var options = new DbContextOptionsBuilder<ProductServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryCategoryDb").Options;

            _context = new ProductServiceDatabaseContext(options);
            _controller = new CategoryController(_context);
            SeedCategoryInMemoryDatabase();
        }

        [Fact]
        public async Task AddProduct_ShouldReturnBadrequestNoDataError()
        {
            IActionResult result = await _controller.AddCategory(default(AddCategoryModel));
            var actionResult = result as BadRequestObjectResult;

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
            Assert.Equal("CATEGORY.ADD.NO_DATA", actionResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnBadrequestNoNameError()
        {
            IActionResult result = await _controller.AddCategory(CreateNewAddCategoryDTO(""));
            var actionResult = result as BadRequestObjectResult;

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
            Assert.Equal("CATEGORY.ADD.NO_NAME", actionResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnBadrequestNameAlreadyExistsError()
        {
            IActionResult result = await _controller.AddCategory(CreateNewAddCategoryDTO("Category1"));
            var actionResult = result as BadRequestObjectResult;

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
            Assert.Equal("PRODUCT.ADD.NAME_ALREADY_EXISTS", actionResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnNewCategoryId()
        {
            IActionResult result = await _controller.AddCategory(CreateNewAddCategoryDTO("Newcategory"));
            var actionResult = result as OkObjectResult;

            var lastEntry = _context.Categories.LastOrDefault();

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
            Assert.Equal(4, actionResult.Value);
            Assert.Equal(lastEntry.Id, actionResult.Value);
        }

        private void SeedCategoryInMemoryDatabase()
        {
            var data = new List<Category>
                {
                    new Category { Id = 1, Name = "Category1" },
                    new Category { Id = 2, Name = "Category2" },
                    new Category { Id = 3, Name = "Category3" },
                };
            _context.Categories.AddRange(data);
            _context.SaveChanges();
        }

        private AddCategoryModel CreateNewAddCategoryDTO(string categoryName)
        {
            AddCategoryModel newCategoryModel = new()
            {
                Name = categoryName
            };
            return newCategoryModel;
        }

        /// <summary>
        /// Disposes handles cleaning of the database entries after each test
        /// </summary>
        public void Dispose()
        {
            _context.Categories.RemoveRange(_context.Categories.ToList());
            _context.SaveChanges();
        }
    }
}
