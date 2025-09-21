namespace GovElec.App.Services;


public interface ICookieService
{
	void Set(string key, string value, CookieOptions options);
	string? Get(string key);
	void Delete(string key);
}

public class CookieService(IHttpContextAccessor httpContextAccessor)
{

	public void Set(string key, string value, CookieOptions options)
	{
		httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, options);
	}

	public string? Get(string key)
	{
		if (httpContextAccessor.HttpContext == null)
			return null;

		if (httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(key, out var value))
			return value;

		return null;
	}

	public void Delete(string key)
	{
		httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
	}
}

