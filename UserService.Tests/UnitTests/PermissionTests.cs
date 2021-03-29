using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Controllers;
using UserService.DBContexts;
using UserService.Models;
using Xunit;

namespace UserService.Tests.UnitTests
{
    public class PermissionTests
    {
        [Fact]
        public async Task GetPermissions_WhenCalled_ReturnListOfPermissions()
        {
            var options = new DbContextOptionsBuilder<UserServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryUserDb").Options;

            var context = new UserServiceDatabaseContext(options);
            SeedPermissionInMemoryDatabaseWithData(context);
            var controller = new PermissionController(context);
            var result = await controller.GetPermissions();

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var permissions = Assert.IsAssignableFrom<IEnumerable<Permission>>(objectresult.Value);

            Assert.Equal(3, permissions.Count());
            Assert.Equal("BBB", permissions.ElementAt(0).Name);
            Assert.Equal("ZZZ", permissions.ElementAt(1).Name);
            Assert.Equal("AAA", permissions.ElementAt(2).Name);
        }

        private static void SeedPermissionInMemoryDatabaseWithData(UserServiceDatabaseContext context)
        {
            var data = new List<Permission>
                {
                    new Permission { Name = "BBB" },
                    new Permission { Name = "ZZZ" },
                    new Permission { Name = "AAA" },
                };
            context.Permissions.AddRange(data);
            context.SaveChanges();

        }
    }
}
