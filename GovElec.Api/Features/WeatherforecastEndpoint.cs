
namespace GovElec.Api.Features;

public class WeatherforecastEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/weatherforecast", () =>
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithTags("Tests")
        .WithName("WeatherForecast")
        .WithSummary("Renvoie une liste de températures aléatoires.")
        .WithDescription("Ce endpoint renvoie une liste de températures aléatoires afin de tester la communication avec l'Api.");
    }

}
