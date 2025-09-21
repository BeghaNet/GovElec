


using Microsoft.AspNetCore.Authorization;

namespace GovElec.Api.Features.Users;

public class GetAllUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", async (AppDbContext dbContext) =>
        {
            var users = await dbContext.Users.ToListAsync();
            if (users == null)
                return Results.NotFound(new List<UserForListResponse>());
            var response= users.Adapt<List<UserForListResponse>>();
            return Results.Ok(response);
        }).RequireAuthorization("AdminOnly") // Only Admins can get all users
          .WithTags("Users")
          .Produces<List<User>>(StatusCodes.Status200OK)
          .WithName("GetAllUsers")
          .WithSummary("Récupère tous les utilisateurs.")
          .WithDescription("This endpoint allows you to retrieve all users in the system.");
    }
}