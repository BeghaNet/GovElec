namespace GovElec.Api.Features.Demands;

public class GetDemandByIdEndpoint:IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/demands/{id:int}", async (int id, AppDbContext context, HttpContext http) =>
        {
            var utilisateur = http.User.Identity?.Name;
            var utilisateurForId = await context.Users.FirstAsync(u => u.UserName.ToUpper() == utilisateur!.ToUpper());
            if (utilisateurForId == null)
            {
                return Results.BadRequest("Utilisateur non trouvé.");
            }
            var utilisateurId = utilisateurForId.Id;
            var role = http.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            DemandeForReadResponse? response = null;
            var demande = await context.Demandes.FirstOrDefaultAsync(d => d.Id == id);
            if (demande == null)
            {
                return Results.NotFound("Demande non trouvée.");
            }

            // var canRead = role == "Admin" ||
            // role == "SuperAdmin" ||
            // (role == "But" && demande.Statut == "En attente d'approbation") ||
            // demande.DemandeurId == utilisateurId; 
            // if (!canRead)
            // {
            //     return Results.Forbid();
            // }

            response = demande.Adapt<DemandeForReadResponse>();
            response.Demandeur = utilisateurForId.FullName;
            response.Equipe = utilisateurForId.Equipe;
            if (response.Technicien != null)
            {
                var technicien = await context.Users.FirstOrDefaultAsync(u => u.Id == demande.TechnicienId);
                if (technicien != null)
                    response.Technicien = technicien.FullName;
            }    
            response.DateCreation=demande.DateCreation;
            return Results.Ok(response);

        })
        .RequireAuthorization()
        .WithTags("Demandes")
        .WithName("GetDemandById")
        .WithSummary("Récupère une demande par son Id.")
        .Produces<DemandeForReadResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden);
    }
}
