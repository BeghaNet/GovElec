


using Microsoft.AspNetCore.Mvc;

namespace GovElec.Api.Features.Users;

public class DeleteUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/users/delete", async ([FromBody]UserForDeleteCommand command, AppDbContext dbContext) =>
        {
            await Task.Yield();
            var user = await dbContext.Users.FindAsync(command.Id);
            if (user == null)
            {
                return Results.NotFound("Utilisateur non trouvé.");
            }
            if (command.Undelete)
                user.IsDeleted = false;
            else
                // Soft delete the user
                user.IsDeleted = true;

            //dbContext.Users.Update(user);
            dbContext.Users.Remove(user); // Soft delete by removing the user from the context
            await dbContext.SaveChangesAsync();

            return Results.Ok("Utilisateur supprimé avec succès.");
        })
          .RequireAuthorization("AdminOnly") // Only Admins can delete users
	   .WithTags("Users")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithName("DeleteUser")
        .WithSummary("Supprime un utilisateur existant.")
        .WithDescription("This endpoint allows you to soft delete an existing user by providing the user's ID.");
        
    }
}
