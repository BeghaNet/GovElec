
using Microsoft.AspNetCore.Mvc;

namespace GovElec.Api.Features.Authorization;

public class LogoutEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("api/logout",async ([FromBody]string refreshToken, ITokenService tokenService) =>
		{

			//http.Response.Cookies.Append("refreshToken", "", new CookieOptions
			//{
			//	HttpOnly = true,
			//	Secure = true,
			//	SameSite = SameSiteMode.Strict,
			//	Expires = DateTime.UtcNow.AddDays(-1)
			//});
			await tokenService.DisableRefreshTokenAsync(refreshToken);
			return Results.Ok();
		}).WithTags("Authentication").WithName("Logout").WithSummary("Déconnecte l'utilisateur en supprimant le cookie de rafraîchissement.").WithDescription("Ce endpoint permet de déconnecter l'utilisateur en supprimant le cookie de rafraîchissement stocké dans le navigateur.");
	}
}
