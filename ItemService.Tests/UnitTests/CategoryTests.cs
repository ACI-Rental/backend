using System;
using System.Collections.Generic;
using Xunit;
using ItemService;
using ItemService.Controllers;
using ItemService.Models;
using ItemService.DBContexts;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace ItemService.Tests.UnitTests
{
    public class CategoryTest
    {
        CategoryController categoryController;
        Mock<ItemServiceDatabaseContext> mockDbContext;

        internal void Setup()
        {
            var data = new List<Category>
            {
                new Category { Name = "BBB" },
                new Category { Name = "ZZZ" },
                new Category { Name = "AAA" },
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Category>>();

            mockDbSet.As<IDbAsyncEnumerable<Category>>()
            .Setup(m => m.GetAsyncEnumerator())
            .Returns(new TestDbAsyncEnumerator<Category>(data.GetEnumerator()));

            mockDbSet.As<IQueryable<Category>>()
            .Setup(m => m.Provider)
            .Returns(new TestDbAsyncQueryProvider<Category>(data.Provider));

            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(data.Expression);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());


            mockDbContext = new Mock<ItemServiceDatabaseContext>();
            mockDbContext.Setup(m => m.Categories).Returns(mockDbSet.Object);
            categoryController = new CategoryController(mockDbContext.Object);
        }

        [Fact]
        public async void GetCategories_WhenCalled_ReturnListOfCategories()
        {
            Setup();
            var categories = await categoryController.GetCategories();

            mockDbContext.Verify((m => m.Categories), Times.Once());

            //Assert.Equal(3, categories.Result.Value.Count());
            //Assert.Equal("AAA", categories.Result.Value[0].Name);
            //Assert.Equal("BBB", categories.Result.Value[1].Name);
            //Assert.Equal("ZZZ", categories.Result.Value[2].Name);
        }
    }
}
