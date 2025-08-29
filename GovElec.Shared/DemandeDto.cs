public class DemandForCreateCommand
{
    //Entête : ne pourra plus être modifiée.
    public string Projet { get; set; } = string.Empty;
    public Guid Demandeur { get; set; }
    public string Batiment { get; set; } = string.Empty;

    //Corps : données techniques
    public string Equipement { get; set; } = string.Empty;
    public string TypeDemande { get; set; } = "Nouvelle demande";
    public string Priorite { get; set; } = string.Empty;
    public string Contexte { get; set; } = string.Empty;
    public string VenantDe { get; set; } = string.Empty;
    public string Disjoncteur { get; set; } = string.Empty;
    public string Tension { get; set; } = "3x400V";
    public string Regime { get; set; } = "IT";
    public string TypeReseau { get; set; } = "N";
    public float Puissance { get; set; } = 0.0f;
    public float CosPhi { get; set; }=0.9f;
    public float Ks { get; set; } = 1f;
    public float Ku { get; set; } = 1f;
    public string CommentaireUtilisateur { get; set; } = string.Empty;
}
public class DemandForUpdateCommand
{
    public int Id { get; set; }
    public string TypeDemande { get; set; } = string.Empty;
    public string Priorite { get; set; } = string.Empty;
    public string Contexte { get; set; } = string.Empty;
    public string VenantDe { get; set; } = string.Empty;
    public string Disjoncteur { get; set; } = string.Empty;
    public string DisjoncteurPrincipal { get; set; } = string.Empty;
    public string Tension { get; set; } = "3x400V";
    public string Regime { get; set; } = "IT";
    public string TypeReseau { get; set; } = "N";
    public float Puissance { get; set; } = 0.0f;
    public float CosPhi { get; set; } = 0.9f;
    public float Ks { get; set; } = 1f;
    public float Ku { get; set; } = 1f;
    public string Statut { get; set; } = "Nouvelle demande";
    public string StatusTechnicien { get; set; } = "Nouvelle demande";
    public string CommentaireUtilisateur { get; set; } = string.Empty;
    public string CommentaireTechnicien { get; set; } = string.Empty;
    public string CommentaireAdministrateur { get; set; } = string.Empty;
}
public class DemandForDeleteCommand
{
    public int Id { get; set; }
}

public class DemandeForReadRequest
{
    //En fonction de la requête, on renverra une liste ou une fiche
    //Si la requête se fait sur l'Id, on renvoie une seule fiche
    //Si la requête est un nom de projet, on renvoie toutes les fiches liées au projet
    public int Id { get; set; } = -1;
    public string Projet { get; set; } = string.Empty;
}
public class DemandeForHistoryRequest
{
    public int Id { get; set; } 
}
public class DemandeForReadResponse
{
    public int Id { get; set; }
    public string Projet { get; set; } = string.Empty;
    public string Demandeur { get; set; } = string.Empty;
    public string Batiment { get; set; } = string.Empty;

    public string TypeDemande { get; set; } = string.Empty;
    public string Priorite { get; set; } = string.Empty;
    public string Contexte { get; set; } = string.Empty;
    public string VenantDe { get; set; } = string.Empty;
    public string Disjoncteur { get; set; } = string.Empty;
    public string DisjoncteurPrincipal { get; set; } = string.Empty;
    public string Tension { get; set; } = "3x400V";
    public string Regime { get; set; } = "IT";
    public string TypeReseau { get; set; } = "N";
    public float Puissance { get; set; } = 0.0f;
    public float CosPhi { get; set; } = 0.9f;
    public float Ks { get; set; } = 1f;
    public float Ku { get; set; } = 1f;
    public string Technicien { get; set; } = string.Empty;
    public string Statut { get; set; } = "Nouvelle demande";
    public string StatusTechnicien { get; set; } = "Nouvelle demande";
    public string CommentaireUtilisateur { get; set; } = string.Empty;
    public string CommentaireTechnicien { get; set; } = string.Empty;
    public string CommentaireAdministrateur { get; set; } = string.Empty;
    public DateOnly DateDemande { get; set; }
    public DateOnly DateReponseAttendue { get; set; }
}

public class DemandForListResponse
{
    public int Id { get; set; }
    public string Demandeur { get; set; } = string.Empty;
    public string Equipe { get; set; } = string.Empty;
    public string Projet { get; set; } = string.Empty;
    public string Batiment { get; set; } = string.Empty;
    public string Priorite { get; set; } = "Basse";
    public string Status { get; set; } = "Nouvelle demande";
    public string StatusTechnicien { get; set; } = "Nouvelle demande";
    public DateOnly DateDemande { get; set; }
    public DateOnly DateReponseAttendue { get; set; }
}

public class DemandForHistoryResponse
{
    public int Id { get; set; }
    public string Projet { get; set; } = string.Empty;
    public string Demandeur { get; set; } = string.Empty;
    public string Batiment { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime DateDemandeApprobation { get; set; }
    public DateTime DateReponseAttendue { get; set; }
    public DateTime DateReponse { get; set; }
    public DateTime DateReponseFinale { get; set; }
    public DateTime DateDebutTravaux { get; set; }
    public DateTime DateCloture { get; set; } 
    public bool IsClotured()=> DateCloture <= DateTime.Now;
}