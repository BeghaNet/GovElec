using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace GovElec.App.Services;

public class TokenService(CookieService cookieService,IHttpClientFactory httpFactory)
{
	private const string tokenKey="token";
	private const string refreskKey = "refreshToken";
	public async Task<string> GetToken()
	{
		var token=cookieService.Get(tokenKey);
		if (string.IsNullOrWhiteSpace(token)||IsExpired(token))
		{
			var refreshed= await TryRefreshToken();
			if (!refreshed)
				return await Task.FromResult( string.Empty);
			token = cookieService.Get(tokenKey);
		}
		return await Task.FromResult(token!);
		
	}
	public async Task<bool> TryRefreshToken()
	{

		var refreshToken = cookieService.Get(refreskKey);
		if (string.IsNullOrEmpty(refreshToken))
			return false;
		var token=cookieService.Get(tokenKey);
		if(string.IsNullOrEmpty(token))
			return false;
		var user=GetClaimsFromToken(token).FirstOrDefault(c=>c.Type.Equals(ClaimTypes.Name));
		if (user == null)
			return false;
		var http = httpFactory.CreateClient("GovElecApi");
		var response = await http.PostAsJsonAsync("api/refresh", new { Token = token, RefreshToken = refreshToken });
		if (!response.IsSuccessStatusCode)
			return false;
		var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
		if (result == null)
			return false;
		StoreToken(result.Token, result.RefreshToken);
		return true;
	}
	public void StoreToken(string token, string refreshToken)
	{
		token = NormalizeToken(token);
		cookieService.Set(tokenKey, token, new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.Now.AddHours(2)
		});
		refreshToken = NormalizeToken(refreshToken);
		cookieService.Set(refreskKey, refreshToken, new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.Now.AddDays(7)
		});
	}
	public void DeleteTokens()
	{
		cookieService.Delete(tokenKey);
		cookieService.Delete(refreskKey);
	}
	public bool IsExpired(string token)
	{
		token = NormalizeToken(token);
		var handler = new JwtSecurityTokenHandler();
		var jwt = handler.ReadToken(token) as JwtSecurityToken;
		return jwt == null || jwt.ValidTo < DateTime.UtcNow;
	}

	public IEnumerable<Claim> GetClaimsFromToken(string token)
	{
		var raw = NormalizeToken(token);
		var handler = new JwtSecurityTokenHandler();
		var jwt = handler.ReadToken(raw) as JwtSecurityToken;
		return jwt?.Claims??Enumerable.Empty<Claim>();
	}

	private static string NormalizeToken(string? token)
	{
		if (string.IsNullOrWhiteSpace(token)) return string.Empty;

		token = token.Trim();

		// 1) En cas de "Bearer x.y.z"
		const string bearer = "Bearer ";
		if (token.StartsWith(bearer, StringComparison.OrdinalIgnoreCase))
			token = token.Substring(bearer.Length).Trim();

		// 2) En cas de guillemets autour du token
		if (token.Length >= 2 && token[0] == '"' && token[^1] == '"')
			token = token[1..^1];

		// 3) Certaines API renvoient "null"
		if (string.Equals(token, "null", StringComparison.OrdinalIgnoreCase))
			return string.Empty;

		return token;
	}
}
