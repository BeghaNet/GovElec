namespace GovElec.Api.Features.Demands;

public class CreateDemandEndpoint:IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/demands", async (
             DemandForCreateCommand command, 
             AppDbContext context,
             IValidator<DemandForCreateCommand>validator) =>
        {
		   //var result = await mediator.Send(command);
		   var validationResult = await validator.ValidateAsync(command);
		   if (!validationResult.IsValid)
		   {
			   var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
			   return Results.BadRequest(new { Errors = errors });
		   }
		   var demand = command.Adapt<Demande>();
            if (demand == null)
            {
                return Results.BadRequest("Les données de la demande sont incomplètes.");
            }
            var index = context.Demandes.Count() + 1;
            demand.Id = index;
            demand.DateCreation = DateTime.Now;
            demand.Statut = "Nouvelle demande";
            demand.StatutTechnicien = "Nouvelle demande";
            await context.Demandes.AddAsync(demand);
            await context.SaveChangesAsync();
            return Results.Created($"/api/demands/{demand.Id}",new{id=demand.Id});
        })
        .RequireAuthorization()
        .WithTags("Demandes")
        .WithName("CreateDemand")
        .WithSummary("Crée une nouvelle une demande.")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status200OK);
    }
}
