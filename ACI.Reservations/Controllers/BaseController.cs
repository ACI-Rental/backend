using System.Security.Authentication;
using System.Security.Claims;
using ACI.Reservations.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Reservations.Controllers
{
    public class BaseController : ControllerBase
    {
        protected AppUser GetUser()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                throw new AuthenticationException("Unable to fetch AppUser: IsAuthenticated = false");
            }

            return new AppUser(User.Identity!.Name!, User.FindFirstValue(ClaimTypes.NameIdentifier), User.FindFirstValue(ClaimTypes.Email));
        }
    }
}
