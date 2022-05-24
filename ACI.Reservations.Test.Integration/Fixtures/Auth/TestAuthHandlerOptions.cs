using Microsoft.AspNetCore.Authentication;

namespace ACI.Reservations.Test.Integration.Fixtures.Auth;

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public string DefaultUserId { get; set; } = null!;
}