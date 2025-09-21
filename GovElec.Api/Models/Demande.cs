namespace GovElec.Api.Models;

public class Demande
{

    //Entête : ne pourra plus être modifiée.
    public int Id { get; set; }
    public string Projet { get; set; } = string.Empty;
    public Guid DemandeurId { get; set; }
    public string Batiment { get; set; } = string.Empty;
    //Entête Status
    public string Statut { get; set; } = string.Empty;
    public string StatutTechnicien { get; set; } = string.Empty;
    //Corps : données techniques
    public string Equipement { get; set; } = string.Empty;
    public string TypeDemande { get; set; } = string.Empty;
    public string Priorite { get; set; } = string.Empty;
    public string Contexte { get; set; } = string.Empty;
    public string VenantDe { get; set; } = string.Empty;
    public string Disjoncteur { get; set; } = string.Empty;
    public string Tension { get; set; } = string.Empty;
    public string Regime { get; set; } = string.Empty;
    public string TypeReseau { get; set; } = string.Empty;
    public float Puissance { get; set; } = 0.0f;
    public float CosPhi { get; set; } = 0.9f;
    public float Ks { get; set; } = 1f;
    public float Ku { get; set; } = 1f;
    // Les commentaires
    public string CommentaireTechnicien { get; set; } = string.Empty;
    public string CommentaireAdministrateur { get; set; } = string.Empty;
    public string CommentaireUtilisateur { get; set; } = string.Empty;
    // Footer : Les données de But
    public Guid TechnicienId { get; set; }
    public string DisjoncteurPrincipal { get; set; } = string.Empty;

    //Les dates
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime DateDemandeApprobation { get; set; }
    public DateTime DateReponseAttendue { get; set; }
    public DateTime DateReponse { get; set; }
    public DateTime DateReponseFinale { get; set; }
    public DateTime DateDebutTravaux { get; set; }
    public DateTime DateCloture { get; set; }

    public bool IsDeleted { get; set; } = false;
    //Les liens
    public User Demandeur { get; set; } = new User();
    public User Technicien { get; set; }= new User();
}
