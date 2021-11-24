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
            Assert.Equal(1, users.ElementAt(0).Id);
            Assert.Equal(1000, users.ElementAt(0).UserInfo.Studentnumber);
            Assert.Equal("AAA", users.ElementAt(0).UserInfo.Name);
            Assert.Null(users.ElementAt(0).BannedUntil);
            Assert.Equal(2, users.ElementAt(1).Id);
            Assert.Equal(new DateTime(2020, 01, 01), users.ElementAt(1).BannedUntil);
            Assert.Equal(5, users.ElementAt(2).Id);
            Assert.Equal(6, users.ElementAt(3).Id);
            Assert.Equal(7, users.ElementAt(4).Id);
            Assert.Equal(10, users.ElementAt(5).Id);
            Assert.Equal(11, users.ElementAt(6).Id);
            Assert.Equal(12, users.ElementAt(7).Id);
            Assert.Equal(15, users.ElementAt(8).Id);
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
            Assert.Equal(1000, resultValue.Users.First().StudentNumber);
            Assert.Equal("AAA", resultValue.Users.First().Name);
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
            Assert.Equal(1010, firstUser.StudentNumber);
            Assert.Equal("Employee", firstUser.Role.Name);
            Assert.Equal("DDD", resultValue.Users.First().Name);
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

        [Fact]
        public async Task BlockActionCall_UnbanUser()
        {
            var controller = Initialize();
            UserBlockActionModel action = new UserBlockActionModel { Action = UserBlockAction.UNBLOCK, userId = 2 };

            var result = await controller.BlockActionCall(action);

            Assert.IsType<OkResult>(result);

            var userList = await controller.GetUsers();
            var objectresult = Assert.IsType<OkObjectResult>(userList.Result);
            var resultValue = Assert.IsAssignableFrom<IEnumerable<User>>(objectresult.Value);
            var user = resultValue.ElementAt(1);

            Assert.Equal(2, user.Id);
            Assert.Null(user.BannedUntil);
        }

        [Fact]
        public async Task BlockActionCall_BanUser()
        {
            var controller = Initialize();
            DateTime futureTime = DateTime.Now.AddDays(7);

            UserBlockActionModel action = new UserBlockActionModel { Action = UserBlockAction.BLOCK, userId = 1, blockUntil = futureTime };

            var result = await controller.BlockActionCall(action);

            Assert.IsType<OkResult>(result);

            var userList = await controller.GetUsers();
            var objectresult = Assert.IsType<OkObjectResult>(userList.Result);
            var resultValue = Assert.IsAssignableFrom<IEnumerable<User>>(objectresult.Value);
            var user = resultValue.ElementAt(0);

            Assert.Equal(1, user.Id);
            Assert.Equal(futureTime, user.BannedUntil);
        }

        [Fact]
        public async Task BlockActionCall_ShouldReturnBadObjectResultIfModelIsNull()
        {
            var controller = Initialize();
            UserBlockActionModel action = null;

            var result = await controller.BlockActionCall(action);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task BlockActionCall_ShouldReturnBadObjectResultIfUserIdInvalid()
        {
            var controller = Initialize();
            DateTime futureTime = DateTime.Now.AddDays(7);

            UserBlockActionModel action = new UserBlockActionModel { Action = UserBlockAction.BLOCK, userId = -1, blockUntil = futureTime };

            var result = await controller.BlockActionCall(action);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task BlockActionCall_ShouldReturnBadObjectResultIfDateNotGiven()
        {
            var controller = Initialize();
            UserBlockActionModel action = new UserBlockActionModel { Action = UserBlockAction.BLOCK, userId = 2};

            var result = await controller.BlockActionCall(action);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task BlockActionCall_ShouldReturnBadObjectResultIfDateInPast()
        {
            var controller = Initialize();
            DateTime pastTime = DateTime.Now.AddDays(-7);

            UserBlockActionModel action = new UserBlockActionModel { Action = UserBlockAction.BLOCK, userId = 2, blockUntil = pastTime };

            var result = await controller.BlockActionCall(action);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task BlockActionCall_ShouldReturnBadObjectResultIfBanningAdmin()
        {
            var controller = Initialize();
            DateTime futureTime = DateTime.Now.AddDays(7);

            UserBlockActionModel action = new UserBlockActionModel { Action = UserBlockAction.BLOCK, userId = 11, blockUntil = futureTime };

            var result = await controller.BlockActionCall(action);

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

            var infos = new List<UserInfo>
            {
                // First three users' userinfo used for the first page
                new UserInfo { Id = 1, Name = "AAA", Studentnumber = 1000},
                new UserInfo { Id = 2, Name = "BBB", Studentnumber = 1001},
                new UserInfo { Id = 3, Name = "CCC", Studentnumber = 1002},

                // Set of userinfo for the second page
                new UserInfo { Id = 4, Name = "DDD", Studentnumber = 1010},
                new UserInfo { Id = 5, Name = "EEE", Studentnumber = 1011},
                new UserInfo { Id = 6, Name = "FFF", Studentnumber = 1012},

                // Set of userinfo for the third page
                new UserInfo { Id = 7, Name = "GGG", Studentnumber = 1020},
                new UserInfo { Id = 8, Name = "HHH", Studentnumber = 1021},
                new UserInfo { Id = 9, Name = "III", Studentnumber = 1022},
            };

            var data = new List<User>
            {
                // First three users used for the first page
                new User { Id = 1, Role = roles[0], UserInfo = infos[0] },
                new User { Id = 2, Role = roles[0], UserInfo = infos[1], BannedUntil = new DateTime(2020, 01, 01) },
                new User { Id = 5, Role = roles[0], UserInfo = infos[2] },

                // Set of users for the second page
                new User { Id = 6, Role = roles[1], UserInfo = infos[3] },
                new User { Id = 7, Role = roles[1], UserInfo = infos[4] },
                new User { Id = 10, Role = roles[1], UserInfo = infos[5] },

                // Set of users for the third page
                new User { Id = 11, Role = roles[2], UserInfo = infos[6] },
                new User { Id = 12, Role = roles[2], UserInfo = infos[7] },
                new User { Id = 15, Role = roles[2], UserInfo = infos[8] }
            };
            if (!context.Users.Any())
            {
                context.Users.AddRange(data);
            }
            context.SaveChanges();

        }
    }
}
