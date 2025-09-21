namespace GovElec.Api.Validations.Users;

public class UpdateUserValidator : AbstractValidator<UserForUpdateCommand>
{
	private const string PasswordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$";
	public UpdateUserValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("L'ID de l'utilisateur est requis.");
		RuleFor(x => x.Equipe)
			.MaximumLength(50).WithMessage("Le nom de l'équipe ne peut pas dépasser 50 caractères.");
		RuleFor(x => x.UserName)
		    .NotEmpty().WithMessage("Le nom d'utilisateur est requis.")
		    .Matches(@"^[A-Za-z]{2,3}\d{4,10}$")
		    .WithMessage("Le nom d'utilisateur n'est pas au format GSK.");
		RuleFor(x => x.FullName)
			.NotEmpty().WithMessage("Le nom complet est requis.")
			.MaximumLength(100).WithMessage("Le nom complet ne peut pas dépasser 100 caractères.");
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("L'email est requis.")
			.EmailAddress().WithMessage("L'email n'est pas valide.")
			.MaximumLength(100).WithMessage("L'email ne peut pas dépasser 100 caractères.");
		RuleFor(x => x.Phone)
			.MaximumLength(15).WithMessage("Le numéro de téléphone ne peut pas dépasser 15 caractères.");
		//Le changement de mot de passe est géré dans un validator séparé
		//La suite a été génerée automatiquement mais n'est pas utilisée pour l'instant
		//When(x => !string.IsNullOrWhiteSpace(x.Password) || !string.IsNullOrWhiteSpace(x.ConfirmPassword) || !string.IsNullOrWhiteSpace(x.OldPassword), () =>
		//{
		//	RuleFor(x => x.OldPassword)
		//		.NotEmpty().WithMessage("L'ancien mot de passe est requis pour changer le mot de passe.");
		//	RuleFor(x => x.Password)
		//		.NotEmpty().WithMessage("Le nouveau mot de passe est requis.")
		//		.Matches(PasswordPattern).WithMessage("Le mot de passe doit contenir au moins 8 caractères, une majuscule, un chiffre et un caractère spécial.");
		//	RuleFor(x => x.ConfirmPassword)
		//		.Equal(x => x.Password).WithMessage("La confirmation du mot de passe ne correspond pas au nouveau mot de passe.");
		//});

	}
}
