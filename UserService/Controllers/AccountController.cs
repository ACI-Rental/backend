using UserService.Models.DTOs;
using UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserService.DBContexts;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserServiceDatabaseContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(UserServiceDatabaseContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTO loginDto)
        {
            if (loginDto.Username == null || loginDto.Password == null) return BadRequest("No user/password filled in");

            var user = await _context.Users.Include(u => u.UserInfo).SingleOrDefaultAsync(x => x.UserInfo.Name == loginDto.Username);
            
            if(user == null)
            {
                return BadRequest("No user found");
            }
           
            if (user.UserInfo.Password != loginDto.Password)
            {
                return Unauthorized("password doesn't match");
            }
            

            var logindata = new UserDto
            {
                Username = user.UserInfo.Name,
                Token = _tokenService.CreateToken(user)
            };

            return Ok(logindata);




        }
        // not yet implemented
        private async Task<bool> UserExists(int studentnr)
        {
            return await _context.Users.AnyAsync(x => x.UserInfo.Studentnumber == studentnr.ToString());
        }


    }
}
