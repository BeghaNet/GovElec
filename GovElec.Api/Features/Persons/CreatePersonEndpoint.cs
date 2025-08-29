
namespace GovElec.Api.Features;

public class CreatePersonEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/persons/create", async (Person command, IPersonService personService) =>
        {
            var person = await personService.Create(command);
            return Results.Created($"/api/persons/{person.Id}", person);
        }).WithTags("Persons")
          .WithName("CreatePerson")
          .WithSummary("Crée une nouvelle personne.")
          .WithDescription("Ce endpoint crée une nouvelle personne dans la base de données.");
    }
}
