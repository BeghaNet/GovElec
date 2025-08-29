namespace GovElec.Shared;

public class Collections
{
    public Dictionary<string, string> StatusDemande = new();
    public Dictionary<string, string> StatusTechnicien = new();

    public Collections()
    {
        StatusDemande.Add("NEW", "Nouvelle demande");
        StatusDemande.Add("WAIT", "En attente d'approbation");
        StatusDemande.Add("INFO", "En attente d'informations");
        StatusDemande.Add("LATE", "Demande retardée");
        StatusDemande.Add("NOPE", "Demande refusée");
        StatusDemande.Add("OK", "Demande acceptée");
        StatusDemande.Add("REM", "Demande acceptée avec remarque");
        StatusDemande.Add("RUN", "Demande en cours d'exécution");
        StatusDemande.Add("END", "Tâche clôturée");
        StatusDemande.Add("LOST", "Tâche annulée");

        StatusTechnicien.Add("NEW", "Nouvelle demande");
        StatusTechnicien.Add("WAIT", "En attente de prise en charge");
        StatusTechnicien.Add("INFO", "Demande en attente d'informations");
        StatusTechnicien.Add("NOPE", "Demande refusée");
        StatusTechnicien.Add("OK", "Demande acceptée");
        StatusTechnicien.Add("REM", "Demande acceptée avec remarque");
        StatusTechnicien.Add("LOST", "Tâche annulée");
    }
}
