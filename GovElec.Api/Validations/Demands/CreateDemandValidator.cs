namespace GovElec.Api.Validations.Demands;

public class CreateDemandValidator:AbstractValidator<DemandForCreateCommand>
{
	
	public CreateDemandValidator()
	{
		RuleLevelCascadeMode = CascadeMode.Stop;
		RuleFor(x => x.Projet)
			.NotEmpty().WithMessage("Le projet est requis.")
			.MaximumLength(20).WithMessage("Le nom du projet ne peut pas dépasser 20 caractères.");
		RuleFor(x => x.Demandeur)
			.NotEmpty().WithMessage("Le nom du demandeur est requis.");
		RuleFor(x => x.Batiment)
			.NotEmpty().WithMessage("Le bâtiment est requis.")
			.MaximumLength(10).WithMessage("Le nom du bâtiment ne peut pas dépasser 10 caractères.");
		RuleFor(x => x.Equipement)
			.MaximumLength(50).WithMessage("Le nom de l'équipement ne peut pas dépasser 50 caractères.");
		RuleFor(x => x.TypeDemande)
			.NotEmpty().WithMessage("Le type de demande est requis.")
			.Must(td => ListeService.TypesDemande.Contains(td)).
				WithMessage("Le type de demande doit être l'un des suivants : " + string.Join(", ", ListeService.TypesDemande))
			.MaximumLength(50).WithMessage("Le type de demande ne peut pas dépasser 50 caractères.");
		RuleFor(x => x.Contexte)
			.MaximumLength(50).WithMessage("Le contexte ne peut pas dépasser 50 caractères.");
		RuleFor(x => x.VenantDe)
			.NotEmpty().WithMessage("Le champ 'Venant de' est requis.")
			.MaximumLength(20).WithMessage("Le champ 'Venant de' ne peut pas dépasser 20 caractères.");
		RuleFor(x => x.Disjoncteur)
			.NotEmpty().WithMessage("Le disjoncteur est requis.")
			.MaximumLength(10).WithMessage("Le nom du disjoncteur ne peut pas dépasser 10 caractères.");
		RuleFor(x => x.Tension)
			.NotEmpty().WithMessage("La tension est requise.")
			.Must(u=>ListeService.Tensions.Contains(u)).
				WithMessage("La tension doit être l'une des suivantes : " + string.Join(", ", ListeService.Tensions))
			.MaximumLength(10).WithMessage("La tension ne peut pas dépasser 10 caractères.");
		RuleFor(x => x.Regime)
			.NotEmpty().WithMessage("Le régime est requis.")
			.Must(r => ListeService.Regimes.Contains(r)).
				WithMessage("Le régime doit être l'une des suivantes : " + string.Join(", ", ListeService.Regimes))
			.MaximumLength(20).WithMessage("Le régime ne peut pas dépasser");
		RuleFor(x => x.TypeReseau)
			.NotEmpty().WithMessage("Le type de réseau est requis.")
			.Must(tr => ListeService.TypeReseaux.Contains(tr)).
				WithMessage("Le type de réseau doit être l'une des suivantes : " + string.Join(", ", ListeService.TypeReseaux))
			.MaximumLength(10).WithMessage("Le type de réseau ne peut pas dépasser 10 caractères.");
		RuleFor(x => x.Puissance)
			.GreaterThan(0).WithMessage("La puissance doit être supérieure à 0 kW.");

	}
}
