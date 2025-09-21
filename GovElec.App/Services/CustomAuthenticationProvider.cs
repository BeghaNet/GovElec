namespace GovElec.App.Services;

public class CustomAuthenticationProvider(TokenService tokenService) : AuthenticationStateProvider
{
	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		var token = await tokenService.GetToken();
		if (string.IsNullOrEmpty(token))
		{
			var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
			return await Task.FromResult(new AuthenticationState(anonymous));
		}
		var identity = new ClaimsIdentity(tokenService.GetClaimsFromToken(token), "jwt");
		var user = new ClaimsPrincipal(identity);
		return await Task.FromResult(new AuthenticationState(user));
	}
	public void MarkUserAsAuthenticated(string token)
	{
		//var handler = new JwtSecurityTokenHandler();
		//var jwtToken = handler.ReadJwtToken(token);
		var identity = new ClaimsIdentity(tokenService.GetClaimsFromToken(token), "jwt");
		var user = new ClaimsPrincipal(identity);
		var authState = Task.FromResult(new AuthenticationState(user));
		NotifyAuthenticationStateChanged(authState);
	}
	public void MarkUserAsLoggedOut()
	{
		var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
		var authState = Task.FromResult(new AuthenticationState(anonymous));
		NotifyAuthenticationStateChanged(authState);
	}
}
