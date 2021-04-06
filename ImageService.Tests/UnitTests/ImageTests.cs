using ImageService.Controllers;
using ImageService.DBContexts;
using ImageService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ImageService.Tests.UnitTests
{
    public class ImageTests
    {
        [Fact]
        public async Task GetImages_WhenCalled_ReturnListOfImages()
        {
            var options = new DbContextOptionsBuilder<ImageServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryImageDb").Options;

            var context = new ImageServiceDatabaseContext(options);
            SeedImageInMemoryDatabaseWithData(context);
            var controller = new ImageController(context);
            //var result = await controller.GetCategories();

            //var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            //var categories = Assert.IsAssignableFrom<IEnumerable<Image>>(objectresult.Value);

            //Assert.Equal(3, categories.Count());
            //Assert.Equal("BBB", Encoding.ASCII.GetString(categories.ElementAt(0).Blob));
            //Assert.Equal("ZZZ", Encoding.ASCII.GetString(categories.ElementAt(1).Blob));
            //Assert.Equal("AAA", Encoding.ASCII.GetString(categories.ElementAt(2).Blob));
        }

        private static void SeedImageInMemoryDatabaseWithData(ImageServiceDatabaseContext context)
        {
            var data = new List<Image>
                {
                    new Image { Blob = Encoding.ASCII.GetBytes("BBB") },
                    new Image { Blob = Encoding.ASCII.GetBytes("ZZZ") },
                    new Image { Blob = Encoding.ASCII.GetBytes("AAA") }
                };
            context.Images.AddRange(data);
            context.SaveChanges();

        }
    }
}
