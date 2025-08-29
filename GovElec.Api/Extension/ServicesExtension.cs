using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using GovElec.Api.Data;
using GovElec.Api.Services;
namespace GovElec.Api.Extension;

public static class ServicesExtension
{
    /// <summary>
    /// Ajoute les services 
    /// </summary>
    /// <param name="services">Les services nécessaires</param>
    /// <param name="assembly">The assembly containing the endpoint definitions.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection ConfigureApi(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddEndpointsApiExplorer();
        services.AddApiEndpoints(typeof(Program).Assembly);
        services.AddAppDbContext(configuration);

        services.Configure<TokenOptions>(configuration.GetSection("Jwt"));
        services.AddSingleton<ITokenService, TokenService>();
        
        services.AddCorsService();
        services.AddOpenApi();
        services.AddSwagger();
        //builder.Services.AddControllers();

        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IPasswordService, PasswordService>();
        return services;
    }

    /// <summary>
    /// Adds API endpoints from the specified assembly to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add endpoints to.</param>
    /// <param name="assembly">The assembly containing the endpoint definitions.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddApiEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                    type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();
        services.TryAddEnumerable(serviceDescriptors);
        return services;
    }
    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        //services.AddOpenApi();
        services.AddSwaggerGen();
        // services.AddSwaggerGen(c=>
        // {
        //     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //     {
        //         Name = "Authorization",
        //         Type = SecuritySchemeType.Http,
        //         Scheme = "Bearer",
        //         BearerFormat = "JWT",
        //         In = ParameterLocation.Header,
        //         Description = "Entrez votre token au format : Bearer {votre_token}"
        //     });
        //     c.AddSecurityRequirement(new OpenApiSecurityRequirement
        //     {
        //         {
        //             new OpenApiSecurityScheme{
        //                 Reference=new OpenApiReference{
        //                     Type=ReferenceType.SecurityScheme,
        //                     Id="Bearer"
        //                 }
        //             },
        //             new string[]{}
        //         }
        //     });
        // c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        // {
        //     Title = "GovElec API",
        //     Version = "v1",
        //     Description = "API for GovElec application"
        // });

        //});
        return services;
    }

    private static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("SQLiteConnection")));
        return services;
    }

    private static IServiceCollection AddCorsService(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            });

        });
        return services;
    }
    
    

}
