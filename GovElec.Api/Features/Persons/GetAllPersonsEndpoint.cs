namespace GovElec.Api.Features;

public class GetAllPersonsEndpoint:IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/persons", async (IPersonService context) =>
        {
            var persons = await context.GetAll();
            return Results.Ok(persons);
        })
        .WithTags("Persons")
        .WithName("GetAllPersons")
        .WithSummary("Renvoie la liste de toutes les personnes.")
        .WithDescription("Ce endpoint renvoie une liste d'objets Person avec des informations fictives.");
    }
}