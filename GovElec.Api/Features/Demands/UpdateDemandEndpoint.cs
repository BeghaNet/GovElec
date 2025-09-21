
namespace GovElec.Api.Features.Demands;

public class UpdateDemandEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/demands/{id:int}", async (
             int id, 
             DemandForUpdateCommand command, 
             HttpContext http, 
             AppDbContext context,
             IValidator<DemandForUpdateCommand>validator) =>
        {
             var validationResult = await validator.ValidateAsync(command);
		   if (!validationResult.IsValid)
		   {
			   var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
			   return Results.BadRequest(new { Errors = errors });
		   }
		   if (id != command.Id)
		   {
			   return Results.BadRequest("L'ID de la demande dans l'URL ne correspond pas à celui du corps de la requête.");
		   }
		   var demand = await context.Demandes.FindAsync(id);
            if (demand == null)
            {
                return Results.NotFound($"La demande avec l'ID {id} n'a pas été trouvée.");
            }
            if (demand.IsDeleted)
            {
                return Results.BadRequest("La demande ne peut pas être mise à jour car elle est marquée comme supprimée.");
            }
            // Vérifier si l'utilisateur a le droit de mettre à jour la demande
            // Récupérer l'utilisateur et son rôle à partir du contexte HTTP
            //Pour pouvoir éditer une demande, l'utilisateur doit être :
            // - l'utilisateur qui a créé la demande (Demandeur) mais il sera limité a certains champs
            // - un administrateur Peux éditer tous les champs sauf l'entête
            // - un super administrateur Peux éditer tous les champs y compris l'entête
            // - un technicien ne peux éditer que les champs de But
            var utilisateur = http.User.Identity?.Name;
            //var role = http.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            var utilisateurForControle = await context.Users.FirstAsync(u => u.UserName.ToUpper() == utilisateur.ToUpper());
            var role = utilisateurForControle.Role;
            var CanEdit = role == "Admin" ||
                            role == "SuperAdmin"||
                            (role == "User" && utilisateur == demand.Demandeur.UserName) ||
                            (role == "But" && demand.Statut=="En attente d'approbation");
            //return Results.Ok(role);
            if (!CanEdit)
            {
                return Results.Forbid();
            }

            // Mettre à jour les propriétés de la demande sans passer par automapper
            // Les données à accès limité
            if (role == "SuperAdmin")
            {
                demand.Id = command.Id;

            }
            if (role == "Admin" || role == "SuperAdmin")
            {
                demand.Projet = command.Projet;
                demand.DemandeurId = command.DemandeurId;
                demand.Batiment = command.Batiment;
                //Gestion status de la fiche
                if (demand.Statut != command.Statut)
                {
                    switch (command.Statut)
                    {
                        case "En attente d'approbation":
                            demand.Statut = "En attente d'approbation";
                            demand.StatutTechnicien = "En attente de prise en charge";
                            demand.DateDemandeApprobation = DateTime.Now;
                            demand.DateReponseAttendue = DateTime.Now.AddDays(7);
                            break;
                        case "Acceptée":
                        case "Acceptée avec remarque":
                        case "Refusée":
                            demand.Statut = command.Statut;
                            demand.DateReponseFinale = DateTime.Now;
                            break;
                        case "En attente d'information":
                            demand.Statut = "En attente d'information";
                            demand.StatutTechnicien="Nouvelle demande";
                            demand.DateDemandeApprobation=new DateTime(9999,12,31);
                            demand.DateReponseAttendue=new DateTime(9999,12,31);

                            break;
                    }
                }
                demand.CommentaireAdministrateur = command.CommentaireAdministrateur;
            }
            if (role == "SuperAdmin" || role == "Admin" || role == "Technicien")
            {
                demand.CommentaireTechnicien = command.CommentaireTechnicien;
                if (demand.StatutTechnicien != command.StatusTechnicien)
                {
                    demand.StatutTechnicien = command.StatusTechnicien;
                    demand.DateReponse = DateTime.Now;
                }
            }
            if (role == "SuperAdmin" || role == "Admin" || role == "User")
            {
                demand.CommentaireUtilisateur = command.CommentaireUtilisateur;
                demand.TypeDemande = command.TypeDemande;
                demand.Priorite = command.Priorite;
                demand.Contexte = command.Contexte;
                demand.VenantDe = command.VenantDe;
                demand.Disjoncteur = command.Disjoncteur;
                demand.Tension = command.Tension;
                demand.Regime = command.Regime;
                demand.TypeReseau = command.TypeReseau;
                demand.Puissance = command.Puissance;
                demand.CosPhi = command.CosPhi;
                demand.Ks = command.Ks;
                demand.Ku = command.Ku;
                if (command.Statut == "Activité en cours de réalisation")
                    demand.Statut = command.Statut;
                    
                if (command.Statut == "Côturée")
                {
                    demand.Statut = command.Statut;
                    demand.DateCloture = DateTime.Now;
                }
            }
            context.Update(demand);
            await context.SaveChangesAsync();
            //On ne retourne rien
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Demandes")
        .WithName("UpdateDemand")
        .WithSummary("Met à jour une demande existante.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden);
    }

}
