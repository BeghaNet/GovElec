


namespace GovElec.Api.Features.Users;

public class GetUserByNameEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/byname/{username}", async (string username, AppDbContext dbContext) =>
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return Results.NotFound("Utilisateur non trouvé.");
            }
            var response = user.Adapt<UserForReadResponse>();
            return Results.Ok(response);
        }).WithTags("Users")
          .Produces<User>(StatusCodes.Status200OK)
          .Produces(StatusCodes.Status404NotFound)
          .WithName("GetUserByName")
          .WithSummary("Récupère les détails d'un utilisateur existant.")
          .WithDescription("This endpoint allows you to retrieve the details of an existing user by providing the username.");
    }
}
