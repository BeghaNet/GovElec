
// namespace GovElec.Api.Features.Demands;

// public class GetDemandsEndpoint : IEndpoint
// {
//     public void MapEndpoint(IEndpointRouteBuilder app)
//     {
//         app.MapGet("/api/demands", async (DemandeForReadRequest request,AppDbContext context,HttpContext http) =>
//         {

//             var response = new List<DemandForListResponse>();
//             //Dans un premier temps, on regarde qui demande.
//             //Si c'est un administrateur ou un superadministrateur, on renvoie toutes les demandes en cours
//             //si c'est un technicien, on renvoie toutes les demandes en attente d'approbation (Déjà traitées ou non)
//             //si c'est un user, on renvoie la liste des demandes pour ce user
//             var utilisateur = http.User.Identity?.Name;
//             var utilisateurForId = await context.Users.FirstAsync(u => u.UserName.ToUpper() == utilisateur!.ToUpper());
//             if (utilisateurForId == null)
//             {
//                 return Results.BadRequest("Utilisateur non trouvé.");
//             }
//             var utilisateurId = utilisateurForId.Id;
//             var role = http.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
//             var statuts = new[] { "Nouvelle demande", "En attente d'approbation", "En attente d'information" };
//             var result = await context.Demandes.ToListAsync();//.Where(d => statuts.Contains(d.Statut)).ToListAsync();
//             switch (role)
//             {
//                 case "Administrateur":
//                 case "SuperAdministrateur":
//                     //Si la personne active à au minimum le rôle administrateur
//                     //Il pourra faire une sélection sur le demandeur request.UserName
//                     //Sur le nom du projet request.Project
//                     //et en fonction de request.GetAll récupérer l'historique complet, quel que soit le statut des fiches sélectionnées précédement
//                     //sinon, il récupère toutes les fiches actives
//                     if (!string.IsNullOrEmpty(request.Username))
//                         result = [.. result.Where(d => d.DemandeurId == utilisateurId)];
//                     if (!string.IsNullOrEmpty(request.Project))
//                         result = [.. result.Where(d => d.Projet.ToUpper() == request.Project.ToUpper())];
//                     if (!request.GetAll)
//                         result= [.. result.Where(d => statuts.Contains(d.Statut))];
//                     response = result.Adapt < List<DemandForListResponse>>();
                    
//                     break;
//                 case "Technicien":
//                     //var result2 = await context.Demandes.Where(d => d.Statut == "En attente d'approbation").ToListAsync();
//                     result = [.. result.Where(d => d.Statut == "En attente d'approbation")];
                    
//                     break;
//                 default:
//                     result = [.. result.Where(d => d.DemandeurId == utilisateurId)];
//                     if (!string.IsNullOrEmpty(request.Project))
//                         result = [.. result.Where(d => d.Projet.ToUpper() == request.Project.ToUpper())];
//                     if (!request.GetAll)
//                         result= [.. result.Where(d => statuts.Contains(d.Statut))];
//                     break;
//             }
//             response = result.Adapt<List<DemandForListResponse>>();
//             if (response.Count == 0)
//             {
//                 return Results.NoContent();
//             }
//             return Results.Ok(response);

//         })
//         .RequireAuthorization()
//         .WithTags("Demandes")
//         .WithName("GetDemands")
//         .WithSummary("Récupère la liste des demandes en cours.")
//         .Produces(StatusCodes.Status204NoContent)
//         .Produces(StatusCodes.Status400BadRequest)
//         .Produces(StatusCodes.Status404NotFound)
//         .Produces(StatusCodes.Status403Forbidden);
//     }
// }
