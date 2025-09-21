public class DemandForCreateCommand
{
	private string _projet = string.Empty;
	private string _batiment = string.Empty;
	//Entête : ne pourra plus être modifiée.
	public string Projet { get=>_projet; set=>_projet=value.ToUpper(); }
    public Guid Demandeur { get; set; }
    public string Batiment { get => _batiment; set => _batiment = value.ToUpper(); }

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
	private string _projet = string.Empty;
	private string _batiment = string.Empty;
	public int Id { get; set; }
	//Ces champs ne seront remplis que si le rôle de l'utilisateur est SuperAdministrateur
	//Si le rôle est Administrateur, seul les champs Projet et bâtiments pourront être modifié.
	public string Projet { get => _projet; set => _projet = value.ToUpper(); }
	public Guid DemandeurId { get; set; }
	public string Batiment { get => _batiment; set => _batiment = value.ToUpper(); }
	public string Equipement { get; set; } = string.Empty;
	// Fin de limitations
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
    public Guid TechnicienId { get; set; }
    //public Guid UserId { get; set; } //Attention : Doit-être l'utilisateur connecté pas le demandeur qui doit rester fixe.
    public string CommentaireUtilisateur { get; set; } = string.Empty;
    public string CommentaireTechnicien { get; set; } = string.Empty;
    public string CommentaireAdministrateur { get; set; } = string.Empty;
    //Dates
    //Date de soumission au service électrique. La réponse sera calculée lors de la soumission
    public DateTime DateSoumission { get; set; }
    //Date de réponse du service électrique
    public DateTime DateReponse { get; set; }
    //Date de réponse de la gouvernance
    public DateTime DateReponseFinale { get; set; }
}
public class DemandForDeleteCommand
{
    public int Id { get; set; }
}

public class DemandeForReadRequest
{
    
    //Si la requete se fait sur le user, on renvoie toutes les fiches du user
    //Si la requête est un nom de projet, on renvoie toutes les fiches liées au projet
    //Si GetAll reste sur false, on ne renvoie que les fiches en cours, sinon on renvoie tout l'historique
    public string Username { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public bool GetAll { get; set; } = false;
   
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
    public string Equipe { get; set; } = string.Empty;
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
    public DateTime DateCreation { get; set; }
    public DateTime DateReponseAttendue { get; set; }
}

public class DemandForListResponse
{
    public int Id { get; set; }
    public string Demandeur { get; set; } = string.Empty;
    public string Projet { get; set; } = string.Empty;
    public string Batiment { get; set; } = string.Empty;
    public string Priorite { get; set; } = "Basse";
    public string Status { get; set; } = "Nouvelle demande";
    public string StatusTechnicien { get; set; } = "Nouvelle demande";
    public DateTime DateCreation { get; set; }
    public DateTime DateReponseAttendue { get; set; }
}

public class DemandForHistoryResponse
{
    public int Id { get; set; }
    public string Projet { get; set; } = string.Empty;
    public string Demandeur { get; set; } = string.Empty;
    public string Batiment { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime DateCreationApprobation { get; set; }
    public DateTime DateReponseAttendue { get; set; }
    public DateTime DateReponse { get; set; }
    public DateTime DateReponseFinale { get; set; }
    public DateTime DateDebutTravaux { get; set; }
    public DateTime DateCloture { get; set; } 
    public bool IsClotured()=> DateCloture <= DateTime.Now;
}