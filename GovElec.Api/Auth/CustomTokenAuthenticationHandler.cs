

using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using GovElec.Api.Services;

namespace GovElec.Api.Auth;

public class CustomTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "CustomToken";
    private readonly ITokenService _tokenService;

    public CustomTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ITokenService tokenService)
        : base(options, logger, encoder)
    {
        //ISystemClock clock,
        //, clock
        _tokenService = tokenService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var auth))
            return Task.FromResult(AuthenticateResult.NoResult());

        var value = auth.ToString();
        if (!value.StartsWith("Custom ", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.NoResult());

        var token = value.Substring("Custom ".Length).Trim();
        var principal = _tokenService.ValidateToken(token);
        if (principal is null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid token"));

        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

