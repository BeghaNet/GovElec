namespace GovElec.Api.Features;

public class DeletePersonEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/persons/delete/{id}", (int id, IPersonService context) =>
        {
            context.Delete(id);

        })
        .WithTags("Persons")
        .WithName("GetPerson")
        .WithSummary("Renvoie les informations d'une personne.")
        .WithDescription("Ce endpoint renvoie un objet Person avec des informations fictives.");
    }
}
