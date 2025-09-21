
namespace GovElec.Api.Features.Users;

public class ChangePasswordEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPatch("/api/users/change-password", async (
			ChangePasswordCommand command, 
			IPasswordService passwordService,
			IValidator<ChangePasswordCommand> validator
			) =>
		{
			var validationResult = await validator.ValidateAsync(command);
			if (!validationResult.IsValid)
			{
				return Results.ValidationProblem(validationResult.ToDictionary());
			}
			//var passwordVerified = await passwordService.VerifyAsync(command.UserName, command.OldPassword);
			//if (!passwordVerified)
			//	return Results.BadRequest("Ancien mot de passe incorrect.");
			//if (string.IsNullOrEmpty(command.NewPassword) || string.IsNullOrEmpty(command.ConfirmPassword) || command.NewPassword != command.ConfirmPassword)
			//	return Results.BadRequest("Nouveau mot de passe manquant ou ne correspondent pas.");
			await passwordService.CreateAsync(command.UserName, command.NewPassword);
			return Results.Ok("Mot de passe changé avec succès.");
		}).RequireAuthorization();
	}
}
