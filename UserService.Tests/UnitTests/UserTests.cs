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
    public class UserTests
    {
        [Fact]
        public async Task GetUsers_WhenCalled_ReturnListOfUsers()
        {
            var options = new DbContextOptionsBuilder<UserServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryUserDb").Options;

            var context = new UserServiceDatabaseContext(options);
            SeedUserInMemoryDatabaseWithData(context);
            var controller = new UserController(context);
            var result = await controller.GetUsers();

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsAssignableFrom<IEnumerable<User>>(objectresult.Value);

            Assert.Equal(3, users.Count());
            Assert.Equal(1, users.ElementAt(0).StudentNumber);
            Assert.Equal(2, users.ElementAt(1).StudentNumber);
            Assert.Equal(5, users.ElementAt(2).StudentNumber);
        }

        private static void SeedUserInMemoryDatabaseWithData(UserServiceDatabaseContext context)
        {
            var data = new List<User>
                {
                    new User { StudentNumber = 1 },
                    new User { StudentNumber = 2 },
                    new User { StudentNumber = 5 }
                };
            context.Users.AddRange(data);
            context.SaveChanges();

        }
    }
}
