
using GovElec.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GovElec.Api.Features.Authorization;

public class RefreshTokendEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("api/refresh-token", async ([FromBody]string refreshToken, AppDbContext context, ITokenService tokenService) =>
		{
			//var refreshToken = http.Request.Cookies["refreshToken"];
			//var refreshToken = refreshToken;
			if (string.IsNullOrWhiteSpace(refreshToken))
				return Results.BadRequest();
			var isValid = await tokenService.IsRefreshTokenValidAsync(refreshToken);
			if (!isValid)
				return Results.Unauthorized();
			var username = await tokenService.GetUserFromRefreshTokenAsync(refreshToken);
			if (string.IsNullOrWhiteSpace(username))
				return Results.Unauthorized();
			await tokenService.DisableRefreshTokenAsync(refreshToken);
			var (newAccessToken, newRefreshToken, exp) = await tokenService.CreateTokenAsync(username);
			return Results.Ok(new TokenResponse(newAccessToken, newRefreshToken, exp));
		}).WithTags("Authentication").WithName("RefreshToken").WithSummary("Renouvelle le token d'accès en utilisant un token de rafraîchissement valide.").WithDescription("Ce endpoint permet de renouveler le token d'accès en fournissant un token de rafraîchissement valide.");
	}
}
