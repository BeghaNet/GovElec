namespace GovElec.App.Services;

public class AccessTokenService(CookieService cookieService)
{
	private const string cookieKey= "accessToken";

	public void SetToken(string token)
	{
		cookieService.Set(cookieKey, token, new CookieOptions
		{
			HttpOnly = false,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.Now.AddHours(2)
		});
	}
	public string? GetToken()
	{
		return cookieService.Get(cookieKey);
	}
	public void DeleteToken()
	{
		cookieService.Delete(cookieKey);
	}
}
