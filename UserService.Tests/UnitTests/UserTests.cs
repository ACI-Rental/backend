using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UserService.Controllers;
using UserService.DBContexts;
using UserService.Models;
using UserService.Models.DTO;
using Xunit;

namespace UserService.Tests.UnitTests
{
    public class UserTests
    {

        /// <summary>
        /// Initializes the test by creating the database and controller
        /// </summary>
        /// <param name="seed">Whether the database should be filled with data</param>
        /// <param name="callerName">The name of the method. 
        /// Gets filled using the CallerMemberName attribute
        /// </param>
        /// <returns>The user controller set up for testing</returns>
        private UserController Initialize(bool seed = true, [CallerMemberName] string callerName = "")
        {
            var options = new DbContextOptionsBuilder<UserServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryUserDb_" + callerName).Options;

            var context = new UserServiceDatabaseContext(options);
            if (seed)
            {
                SeedUserInMemoryDatabaseWithData(context);
            }

            return new UserController(context);
        }

        [Fact]
        public async Task GetUsers_WhenCalled_ReturnListOfUsers()
        {
            var controller = Initialize();
            var result = await controller.GetUsers();

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsAssignableFrom<IEnumerable<User>>(objectresult.Value);

            Assert.Equal(9, users.Count());
            Assert.Equal(1, users.ElementAt(0).StudentNumber);
            Assert.Equal(2, users.ElementAt(1).StudentNumber);
            Assert.Equal(5, users.ElementAt(2).StudentNumber);
            Assert.Equal(6, users.ElementAt(3).StudentNumber);
            Assert.Equal(7, users.ElementAt(4).StudentNumber);
            Assert.Equal(10, users.ElementAt(5).StudentNumber);
            Assert.Equal(11, users.ElementAt(6).StudentNumber);
            Assert.Equal(12, users.ElementAt(7).StudentNumber);
            Assert.Equal(15, users.ElementAt(8).StudentNumber);
        }

        [Fact]
        public async Task GetUsersPage_ShouldReturnFirstPageIfIndexZero()
        {
            var controller = Initialize();
            var result = await controller.GetUsersPage(0, 3);

            var actionResult = Assert.IsType<OkObjectResult>(result);

            var resultValue = Assert.IsType<UsersPage>(actionResult.Value);

            Assert.Equal(3, resultValue.Users.Count());
            Assert.Equal(0, resultValue.CurrentPage);
            Assert.Equal(1, resultValue.Users.First().StudentNumber);
        }

        [Fact]
        public async Task GetUsersPage_ShouldReturnSecondPageIfIndexOne()
        {
            var controller = Initialize();
            var result = await controller.GetUsersPage(1, 3);

            var actionResult = Assert.IsType<OkObjectResult>(result);

            var resultValue = Assert.IsType<UsersPage>(actionResult.Value);

            Assert.Equal(3, resultValue.Users.Count());
            Assert.Equal(1, resultValue.CurrentPage);

            var firstUser = resultValue.Users.First();
            Assert.Equal(6, firstUser.StudentNumber);
            Assert.Equal("Employee", firstUser.Role.Name);
        }

        [Fact]
        public async Task GetUsersPage_ShouldReturnAllUsers()
        {
            var controller = Initialize();
            var result = await controller.GetUsersPage(0, 100);

            var actionResult = Assert.IsType<OkObjectResult>(result);

            var resultValue = Assert.IsType<UsersPage>(actionResult.Value);

            Assert.Equal(9, resultValue.Users.Count());
            Assert.Equal(9, resultValue.TotalUsersCount);
            Assert.Equal(0, resultValue.CurrentPage);
        }

        [Fact]
        public async Task GetUsersPage_ShouldReturnLastPage()
        {
            var controller = Initialize();
            var result = await controller.GetUsersPage(50, 3);

            var actionResult = Assert.IsType<OkObjectResult>(result);

            var resultValue = Assert.IsType<UsersPage>(actionResult.Value);

            Assert.Equal(3, resultValue.Users.Count());
            Assert.Equal(9, resultValue.TotalUsersCount);
            Assert.Equal(2, resultValue.CurrentPage);
        }

        [Fact]
        public async Task GetUsersPage_ShouldReturnBadObjectResultIfPageSizeNegative()
        {
            var controller = Initialize(seed: false);
            var result = await controller.GetUsersPage(0, -3);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetUsersPage_ShouldReturnBadObjectResultIfPageIndexIsNegative()
        {
            var controller = Initialize(seed: false);
            var result = await controller.GetUsersPage(-3, 1);

            Assert.IsType<BadRequestObjectResult>(result);
        }



        private static void SeedUserInMemoryDatabaseWithData(UserServiceDatabaseContext context)
        {
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "User" },
                new Role { Id = 2, Name = "Employee" },
                new Role { Id = 3, Name = "Admin" }
            };
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(roles);
            }

            var data = new List<User>
            {
                // First three users used for the first page
                new User { StudentNumber = 1, Role = roles[0] },
                new User { StudentNumber = 2, Role = roles[0] },
                new User { StudentNumber = 5, Role = roles[0] },

                // Set of users for the second page
                new User { StudentNumber = 6, Role = roles[1] },
                new User { StudentNumber = 7, Role = roles[1] },
                new User { StudentNumber = 10, Role = roles[1] },

                // Set of users for the third page
                new User { StudentNumber = 11, Role = roles[2] },
                new User { StudentNumber = 12, Role = roles[2] },
                new User { StudentNumber = 15, Role = roles[2] }
            };
            if (!context.Users.Any())
            {
                context.Users.AddRange(data);
            }
            context.SaveChanges();

        }
    }
}
