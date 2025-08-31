
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace GovElec.App.Security;

public class JWTAuthenticationHandler(IOptionsMonitor<CustomOptions> options, 
	ILoggerFactory logger, UrlEncoder encoder)
	: AuthenticationHandler<CustomOptions>
	(options, logger, encoder)
{
	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		try
		{
			var token=Request.Cookies["accessToken"];
			if (string.IsNullOrWhiteSpace(token))
				return await Task.FromResult(AuthenticateResult.NoResult());
			var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
			var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
			var user = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(user, Scheme.Name);
			return await Task.FromResult(AuthenticateResult.Success(ticket));
		}
		catch (Exception ex)
		{
			var msg = ex.Message;
		}
			// Since authentication is handled by the JWTAuthenticationStateProvider, we can return NoResult here.
			return await Task.FromResult(AuthenticateResult.NoResult());
	}
	protected override Task HandleChallengeAsync(AuthenticationProperties properties)
	{
		//Response.StatusCode = 401;
		Response.Redirect("/login");
		return Task.CompletedTask;
	}
	protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
	{
		Response.StatusCode = 403;
		Response.Redirect("/login");
		return Task.CompletedTask;
	}
}
public class CustomOptions:AuthenticationSchemeOptions
{
	
}