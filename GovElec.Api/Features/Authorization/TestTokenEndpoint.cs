
namespace GovElec.Api.Features.Authorization;

public class TestTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/test/{token}", (string token,ITokenService tokenService) =>
        {
            var result = tokenService.ValidateToken(token);
            if (result != null)
                return Results.Ok($"{result.Identity?.Name} - {string.Join(",", result.Claims.Select(c => c.Type + "=" + c.Value))}");
            return Results.Unauthorized();
        });
    }
}
