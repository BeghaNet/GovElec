namespace GovElec.Api.Validations.Users;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
	private readonly IPasswordService? passwordService;
	private const string PasswordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$";
	public ChangePasswordValidator(IPasswordService passwordService)
	{
		this.passwordService = passwordService;

		RuleFor(x => x.UserName)
		    .NotEmpty().WithMessage("Le nom d'utilisateur est requis.")
		    .Matches(@"^[A-Za-z]{2,3}\d{4,10}$")
		    .WithMessage("Le nom d'utilisateur n'est pas au format GSK.");
		RuleFor(x => x.OldPassword)
			.NotEmpty().WithMessage("L'ancien mot de passe est requis.");
		RuleFor(x => x)
			  .MustAsync(async (command, cancellation) =>
			  {
				  return await passwordService.VerifyAsync(command.UserName, command.OldPassword);
			  })
			  .WithMessage("Le mot de passe actuel est incorrect.");
		RuleFor(x => x.NewPassword)
			.NotEmpty().WithMessage("Le nouveau mot de passe est requis.")
			.Matches(PasswordPattern).WithMessage("Le mot de passe doit contenir au moins 8 caractères, une majuscule, un chiffre et un caractère spécial.");
		RuleFor(x => x.ConfirmPassword)
			.Equal(x => x.NewPassword).WithMessage("La confirmation du mot de passe ne correspond pas au nouveau mot de passe.");

		
	}

}