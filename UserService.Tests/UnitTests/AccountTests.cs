using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UserService.Controllers;
using UserService.DBContexts;
using UserService.Models;
using UserService.Models.DTO;
using UserService.Models.DTOs;
using Xunit;

namespace UserService.Tests.UnitTests
{
    public class AccountTests
    {

        private AccountController Initialize(bool seed = true, [CallerMemberName] string callerName = "")
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
            var config = builder.Build();

            var options = new DbContextOptionsBuilder<UserServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryAccountDb_" + callerName).Options;
            
            var tokenService = new TokenService(config);

            var context = new UserServiceDatabaseContext(options);
            if (seed)
            {
                SeedUserInMemoryDatabaseWithData(context);
            }

            return new AccountController(context, tokenService);
        }


        [Fact]
        public async Task Login_WhenCalled_ReturnUserDTO()
        {
            var controller = Initialize();
            var loginDto = new LoginDTO
            {
                Password = "A1",
                Username = "AAA"
            };

            var result = await controller.Login(loginDto);

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsAssignableFrom<UserDto>(objectresult.Value);

            Assert.Equal("AAA", user.Username);
            Assert.NotEmpty(user.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnBadObjectResultIfUsernameNull()
        {
            var controller = Initialize();
            var loginDto = new LoginDTO
            {
                Password = "A1"
            };

            var result = await controller.Login(loginDto);

            var objectresult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("No user/password filled in", objectresult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnBadObjectResultIfPasswordNull()
        {
            var controller = Initialize();
            var loginDto = new LoginDTO
            {
                Username = "AAA"
            };

            var result = await controller.Login(loginDto);

            var objectresult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("No user/password filled in", objectresult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnBadObjectResultIfUserNotExist()
        {
            var controller = Initialize();
            var loginDto = new LoginDTO
            {
                Username = "CYR",
                Password = "A1"
            };

            var result = await controller.Login(loginDto);

            var objectresult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("No user found", objectresult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorizedIfPasswordIncorrect()
        {
            var controller = Initialize();
            var loginDto = new LoginDTO
            {
                Username = "AAA",
                Password = "DFGH"
            };

            var result = await controller.Login(loginDto);

            var objectresult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Password does not match", objectresult.Value);
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
                new UserInfo { Id = 1, Name = "AAA", Password = "A1", Studentnumber = "1000"},
                new UserInfo { Id = 2, Name = "BBB", Password = "B2"},
                new UserInfo { Id = 3, Name = "CCC", Password = "C3"},
            };

            var data = new List<User>
            {
                // First three users used for the first page
                new User { Id = 1, Role = roles[0], UserInfo = infos[0] },
                new User { Id = 2, Role = roles[1], UserInfo = infos[1] },
                new User { Id = 5, Role = roles[2], UserInfo = infos[2] },
            };
            if (!context.Users.Any())
            {
                context.Users.AddRange(data);
            }
            context.SaveChanges();

        }
    }
}
