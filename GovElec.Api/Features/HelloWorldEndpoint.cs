namespace GovElec.Api.Features;

public class HelloWorldEndpoint:IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/hello", () => "Hello, World!")
            .WithName("HelloWorld")
            .WithSummary("Renvoie un message d'acceuil.")
            .WithDescription("Ce endpoint renvoie un simple message 'Hello, World!'.");
    }
}