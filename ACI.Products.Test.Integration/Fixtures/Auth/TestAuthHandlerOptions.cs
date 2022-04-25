using Microsoft.AspNetCore.Authentication;

namespace ACI.Products.Test.Integration.Fixtures.Auth;

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public string DefaultUserId { get; set; } = null!;
}

