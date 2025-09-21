namespace GovElec.Api.Extension;

public static class ApplicationExtension
{
    public static IApplicationBuilder ConfigureApplication(this WebApplication app)
    {
        // Add your application configuration logic here
        // For example, you can add middleware, configure services, etc.
        app.UseHttpsRedirection();
        app.UseRouting();
        app.MapEndpoints();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapOpenApi();
        app.MapDocumentation();
        app.UseCors();
        return app;
    }
    private static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services
            .GetRequiredService<IEnumerable<IEndpoint>>();
        IEndpointRouteBuilder builder =
            routeGroupBuilder is null ? app : routeGroupBuilder;
        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
    private static IApplicationBuilder MapDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            //app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(); //options=>{
            //     options.SwaggerEndpoint("/swagger/v1/swagger.json", "GovElec API V1");
            //     options.RoutePrefix = string.Empty; // Serve Swagger at the app's root
            // });
        }
        return app;
    }
}
