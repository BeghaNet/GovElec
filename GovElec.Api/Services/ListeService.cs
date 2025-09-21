namespace GovElec.Api.Services;

public static class ListeService
{
	public static readonly List<string> Roles = new()
	{
		"SuperAdmin",
		"Admin",
		"But",
		"User"
	};
	public static readonly List<string> Statuts = new()
	{
		"Nouvelle demande",
		"En attente d'approbation",
		"En attente d'informations",
		"Rejetée",
		"Approuvée",
		"Approuvée avec remarques",
		"En cours de réalisation",
		"Clôturée"
	};
	public static readonly List<string> StatutsTechnicien = new()
	{
		"Nouvelle demande",
		"En attente de prise en charge",
		"Attribuée",
		"Acceptée",
		"Acceptée avec remarque",
		"Rejetée",
	};
	public readonly static List<string> Priorites = new()
	{
		"Critique",
		"Haute",
		"Moyenne",
		"Basse"
	};
	public readonly static List<string> TypesDemande = new()
	{
		"Nouvelle installation",
		"Modification d'installation existante",
		"Déconnexion temporaire",
		"Déconnexion permanente",
		"Inspection et maintenance",
		"Autre"
	};
	public static readonly List<string> Regimes = new()
	{
		"IT",
		"ITAN",
		"TT",
		"TN-S",
		"TN-C",
		"Autre"
	};
	public static readonly List<string> Tensions = new()
	{
		"11 kV",
		"3x400V",
		"3x400V + N",
		"3x230V",
		"230V",
		"24V",
		"Autre"
	};
	public static readonly List<string> TypeReseaux = new()
	{
		"N",
		"NS",
		"No Break",
		"Autre"
	};

}
