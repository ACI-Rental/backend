using UserService.Models;

namespace UserService.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
