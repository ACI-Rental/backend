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
    public class RoleTests
    {
        [Fact]
        public async Task GetRoles_WhenCalled_ReturnListOfRoles()
        {
            var options = new DbContextOptionsBuilder<UserServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryUserDb").Options;

            var context = new UserServiceDatabaseContext(options);
            SeedRoleInMemoryDatabaseWithData(context);
            var controller = new RoleController(context);
            var result = await controller.GetRoles();

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var categories = Assert.IsAssignableFrom<IEnumerable<Role>>(objectresult.Value);

            Assert.Equal(3, categories.Count());
            Assert.Equal("BBB", categories.ElementAt(0).Name);
            Assert.Equal("ZZZ", categories.ElementAt(1).Name);
            Assert.Equal("AAA", categories.ElementAt(2).Name);
        }

        private static void SeedRoleInMemoryDatabaseWithData(UserServiceDatabaseContext context)
        {
            var data = new List<Role>
                {
                    new Role { Name = "BBB" },
                    new Role { Name = "ZZZ" },
                    new Role { Name = "AAA" },
                };
            context.Roles.AddRange(data);
            context.SaveChanges();

        }
    }
}
