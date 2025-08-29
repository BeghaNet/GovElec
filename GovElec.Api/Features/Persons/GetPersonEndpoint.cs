
namespace GovElec.Api.Features;

// public class GetPersonEndpoint : IEndpoint
// {
//     public void MapEndpoint(IEndpointRouteBuilder app)
//     {
//         app.MapGet("/api/persons/{id:int}", async (int id, IPersonService context) =>
//         {
//             await Task.Yield(); // Simulate async operation
//             // var person = await context.GetById(id);
//             // if (person == null)
//             // {
//             //     return Results.NotFound();
//             // }
//             // return Results.Ok(person);
//             return Results.Ok($"Test:{id}");
//         })
//         .WithTags("Persons")
//         .WithName("GetPerson")
//         .WithSummary("Renvoie les informations d'une personne.")
//         .WithDescription("Ce endpoint renvoie un objet Person avec des informations fictives.");
//     }
// }
public class GetPersonByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/persons/get/{id:int}", async (int id, IPersonService context) =>
        {
            await Task.Yield();
            var person = await context.GetById(id);
            if (person == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(person);
        })
        .WithTags("Persons")
        .WithName("GetPersonsById")
        .WithSummary("Renvoie la liste de toutes les personnes.")
        .WithDescription("Ce endpoint renvoie une liste d'objets Person avec des informations fictives.");
    }

}