


using FluentValidation;

namespace GovElec.Api.Features.Users;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/create", async 
             (
               UserForCreateCommand command, 
               AppDbContext dbContext,
               IPasswordService passwordService,
               IValidator<UserForCreateCommand> validator
		   ) =>
        {
		   var validationResult = await validator.ValidateAsync(command);
		   if (!validationResult.IsValid)
		   {
			   return Results.ValidationProblem(validationResult.ToDictionary());
		   }
		   // Map the command to a User entity
		   var user = command.Adapt<User>();
             user.UserName = user.UserName.ToUpper();
		   if (user == null)
            {
                return Results.BadRequest("Les données sont incomplètes.");
            }
            if (command.Password != command.ConfirmPassword)
            {
                return Results.BadRequest("Le mot de passe et la confirmation du mot de passe ne correspondent pas.");
            }

            // Check if the user already exists
            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == user.UserName || u.Email == user.Email);

            if (existingUser != null)
            {
                return Results.Conflict("Un utilisateur avec le même nom d'utilisateur ou le même email existes déjà.");
            }
            user.Role = command.Role;
            user.IsDeleted = false; // Default not deleted
            user.Id = Guid.NewGuid(); // Generate a new GUID for the user ID
            
            // Add the new user to the database
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            await passwordService.CreateAsync(command.UserName, command.Password);
            
            return Results.Created($"/api/users/{user.Id}", user);
        }).WithTags("Users")
          .Produces<User>(StatusCodes.Status201Created)
          .Produces(StatusCodes.Status400BadRequest)
          .Produces(StatusCodes.Status409Conflict)
          .WithName("CreateUser")
          .WithSummary("Crée un nouvel utilisateur.")
          .WithDescription("This endpoint allows you to create a new user by providing the necessary user details.");
    }

}
