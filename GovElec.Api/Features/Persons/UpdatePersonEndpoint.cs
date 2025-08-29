
namespace GovElec.Api.Features;

public class UpdatePersonEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/persons/{id:int}", async (int id,Person command, IPersonService personService) =>
        {
            var person = await personService.Update(command);
            if (person == null)
            {
                return Results.NotFound();
            }
            return Results.NoContent();
        }).WithTags("Persons")
          .WithName("UpdatePerson")
          .WithSummary("Met à jour les informations d'une personne.")
          .WithDescription("Ce endpoint met à jour les informations d'une personne existante dans la base de données.");
    }
}