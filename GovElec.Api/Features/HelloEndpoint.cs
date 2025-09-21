
namespace GovElec.Api.Features;

public class HelloEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hello", () =>
        {
            return Results.Ok("Hello, World!");
        })
        .WithTags("Hello");
    }

}
