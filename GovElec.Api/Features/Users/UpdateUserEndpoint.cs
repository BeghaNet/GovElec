


namespace GovElec.Api.Features.Users;

public class UpdateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/update/{id:guid}", async (Guid id, UserForUpdateCommand command, AppDbContext dbContext,IPasswordService passwordService) =>
        {

            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return Results.NotFound("Utilisateur non trouvé.");
            }

            // Update user details : Ne pas utiliser Mapster car si le Password est vide, mapster va écraser le mot de passe existant
            user.UserName = command.UserName.ToUpper();
            user.FullName = command.FullName;
            user.Equipe = command.Equipe;
            user.Email = command.Email;
            user.Phone = command.Phone;
            user.Role = command.Role;
            user.IsDeleted = command.IsDeleted;
            dbContext.Update(user);
            await dbContext.SaveChangesAsync();
            if (!string.IsNullOrEmpty(command.OldPassword))
            {
                var passwordVerified = await passwordService.VerifyAsync(command.UserName, command.OldPassword);
                if (passwordVerified)
                    if (string.IsNullOrEmpty(command.Password) || string.IsNullOrEmpty(command.ConfirmPassword) || command.Password != command.ConfirmPassword)
                        return Results.BadRequest("Mot de passe manquant ou ne correspondent pas.");
                await passwordService.CreateAsync(command.UserName, command.Password);
            }
            
            var response= user.Adapt<UserForReadResponse>();
            return Results.Ok(response);
        }).RequireAuthorization("SelfOrAdmin") // Only Admins or the user themselves can update user
		.WithTags("Users")
          .RequireAuthorization()
		.Produces<User>(StatusCodes.Status200OK)
          .Produces(StatusCodes.Status404NotFound)
		.Produces(StatusCodes.Status400BadRequest)
          .Produces(StatusCodes.Status401Unauthorized)
		.WithName("UpdateUser")
          .WithSummary("Met à jour les détails d'un utilisateur existant.")
          .WithDescription("This endpoint allows you to update the details of an existing user by providing the user's ID and the new details.");
    }
}
