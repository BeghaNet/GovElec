

using Microsoft.AspNetCore.Mvc;

namespace GovElec.Api.Features.Authorization;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/login",async([FromBody]LoginRequest request,AppDbContext context,ITokenService tokenService,IPasswordService passwordService)=>
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName.ToUpper() == request.Username.ToUpper());
            if (user == null) return Results.NotFound();
            if (user.IsDeleted) return Results.BadRequest("L'utilisateur est supprimé.");
            if (string.IsNullOrWhiteSpace(request.Password)) return Results.BadRequest("Le mot de passe ne peut pas être vide.");
            // var loginPassword=request.Password.Trim();
            // var storedPassword = user.PasswordHash.Trim();
            // var tryPassword = BCrypt.Net.BCrypt.Verify( request.Password.Trim(),storedPassword.Trim());
            var tryPassword=await passwordService.VerifyAsync(request.Username, request.Password);
            
            if (!tryPassword) return Results.BadRequest("Le mot de passe ne correspond pas.");

            // Creation du token
            var (accessToken , refreshToken, exp) = await tokenService.CreateTokenAsync(request.Username);

            return Results.Ok(new TokenResponse(accessToken, refreshToken, exp));

        }).WithTags("Authentication");
    }

}
