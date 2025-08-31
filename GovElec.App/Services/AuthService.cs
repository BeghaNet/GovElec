//using GovElec.App.Security;

namespace GovElec.App.Services;

public class AuthService(
	IHttpClientFactory httpClientFactory,
	TokenService tokenService,CustomAuthenticationProvider authProvider)
{
	public async Task<bool>Login(string username, string password)
	{
		var http=httpClientFactory.CreateClient("GovElecApi");
		var response = await http.PostAsJsonAsync("api/login", new { username, password });
		if(!response.IsSuccessStatusCode)
			return false;
		var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
		if(result== null)
			return false;
		tokenService.StoreToken(result.Token, result.RefreshToken);
		authProvider.MarkUserAsAuthenticated(result.Token);
		return true;
		

	}
	public async Task Logout()
	{

		tokenService.DeleteTokens();
		authProvider.MarkUserAsLoggedOut();
		await Task.CompletedTask;
	}
	public async Task<bool> Refresh()
	{
		//var token = tokenService.GetToken();
		//if (string.IsNullOrWhiteSpace(token))
		//	return false;

		//var client = httpClientFactory.CreateClient("GovElecApi");

		//var status = await client.PostAsJsonAsync("api/refresh",token);

		//if (!status.IsSuccessStatusCode)
		//{
		//	tokenService.DeleteToken();
		//	return false;
		//}
		//else
		//{
		//	//var token = await status.Content.ReadAsStringAsync();
		//	var result = JsonConvert.DeserializeObject<TokenResponse>(token);
		//	var tokenResponse = await status.Content.ReadFromJsonAsync<TokenResponse>();
		//	if (tokenResponse != null)
		//	{
		//		tokenService.SetToken(tokenResponse.Token);
		//		return true;
		//	}
		//	else
		//	{
		//		return false;
		//	}
		//}
		return await Task.FromResult(true);
	}

}
