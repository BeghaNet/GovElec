namespace GovElec.Api.Features.Demands;

public class GetDemandsByStatutEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/demands/{statut}", async (string statut, AppDbContext context, HttpContext http) =>
        {
            if (string.IsNullOrEmpty(statut))
                return Results.BadRequest();

            var utilisateur = http.User.Identity?.Name;
            var utilisateurForId = await context.Users.FirstAsync(u => u.UserName.ToUpper() == utilisateur!.ToUpper());
            if (utilisateurForId == null)
            {
                return Results.BadRequest("Utilisateur non trouvé.");
            }
            var utilisateurId = utilisateurForId.Id;
            var role = http.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            var result = await context.Demandes.Where(d => d.Statut == statut).ToListAsync();
            if (role == "User")
                result = [.. result.Where(d => d.DemandeurId == utilisateurId)];
            var response = result.Adapt<List<DemandForListResponse>>();
            if (response.Count == 0)
                return Results.NoContent();
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Demandes")
        .WithName("GetDemandsByStatut")
        .WithSummary("Récupère la liste des demandes en fonction de leur statut.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden);
    }
}
