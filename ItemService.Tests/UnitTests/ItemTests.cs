using System;
using Xunit;
using ItemService;
using ItemService.Controllers;
using ItemService.Models;
using ItemService.DBContexts;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ItemService.Tests.UnitTests
{
    public class ItemTest
    {
        ItemController itemController;
        Mock<ItemServiceDatabaseContext> mockDbContext;

        internal void Setup()
        {
            //var itemMock = new Mock<Item>();
            //itemMock.Setup(item => item.Name).Returns("Canon EOS R5");

            //List<Item> items = new List<Item>()
            //{
            //    itemMock.Object
            //};

            var mockDbSet = new Mock<DbSet<Item>>();

            mockDbContext = new Mock<ItemServiceDatabaseContext>();
            mockDbContext.Setup(m => m.Items).Returns(mockDbSet.Object);
            itemController = new ItemController(mockDbContext.Object);
        }

        [Fact]
        public void DoesItems_ExistInDatabase_ReturnNotNull()
        {
            Setup();
            var result = itemController.GetItems();

            mockDbContext.Verify((m => m.Items), Times.Once());

            Assert.NotNull(result);
        }
    }
}
