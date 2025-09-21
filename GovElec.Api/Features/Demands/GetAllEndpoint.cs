
namespace GovElec.Api.Features.Demands;

public class GetAllEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/demands", async (string? username, string? project, bool? all, AppDbContext context,HttpContext http) =>
        {
            //var response = new List<DemandForListResponse>();
            //Dans un premier temps, on regarde qui demande (la personne connectée).
            // Ce n'est pas la personne dans le username si celui-ci n'est pas vide
            //Si c'est un administrateur ou un superadministrateur, on renvoie toutes les demandes en cours
            //si c'est un technicien, on renvoie toutes les demandes en attente d'approbation (Déjà traitées ou non)
            //si c'est un user, on renvoie la liste des demandes pour ce user
            var utilisateur = http.User.Identity?.Name;
            
            var utilisateurForId = await context.Users.FirstAsync(u => u.UserName.ToUpper() == utilisateur!.ToUpper());
            if (utilisateurForId == null)
            {

                return Results.BadRequest("Utilisateur non trouvé.");
            }

            var utilisateurId = utilisateurForId.Id;
            var role = utilisateurForId.Role;
            var usernameId = Guid.Empty;
            if (!string.IsNullOrEmpty(username))
            {
                var usernameForId = await context.Users.FirstAsync(u => u.UserName.ToUpper() == username!.ToUpper());
                usernameId = usernameForId.Id;
            }

            if (all == null) all = false;
            //var role = http.User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
            //return Results.Ok(role);
            var statuts = new[] { "Nouvelle demande", "En attente d'approbation", "En attente d'information" };
            //On récupère toutes les demandes
            var result = await context.Demandes.ToListAsync();
            //return Results.Ok(result);
            switch (role)
            {
                case "Admin":
                case "SuperAdmin":
                    //Si la personne active à au minimum le rôle administrateur
                    //Il pourra faire une sélection sur le demandeur request.UserName
                    //Sur le nom du projet request.Project
                    //et en fonction de request.GetAll récupérer l'historique complet, quel que soit le statut des fiches sélectionnées précédement
                    //sinon, il récupère toutes les fiches actives
                    if (!string.IsNullOrEmpty(username))
                        result = [.. result.Where(d => d.DemandeurId == usernameId)];
                    if (!string.IsNullOrEmpty(project))
                        result = [.. result.Where(d => d.Projet.ToUpper() == project.ToUpper())];
                    if (all == false)
                        result = [.. result.Where(d => statuts.Contains(d.Statut))];
                    break;
                case "But":
                    //Ne peut afficher que les fiches en attente d'approbation
                    result = [.. result.Where(d => d.Statut == "En attente d'approbation")];
                    break;
                default:
                    //Un utilisateur ne peut voir que ses fiches
                    //Il peut filtrer sur le projet et demander l'historique complet ou non
                    result = [.. result.Where(d => d.DemandeurId == utilisateurId)];
                    if (!string.IsNullOrEmpty(project))
                        result = [.. result.Where(d => d.Projet.ToUpper() == project.ToUpper())];
                    if (all == false)
                        result = [.. result.Where(d => statuts.Contains(d.Statut))];
                    break;
            }
            var response = result.Adapt<List<DemandForListResponse>>();
            if (response.Count == 0)
            {
                return Results.NoContent();
            }
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Demandes")
        .WithName("GetDemands")
        .WithSummary("Récupère la liste des demandes en cours.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden);
    }

}
