using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using GovElec.Api.Data;
using GovElec.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
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
		services.AddScoped<ITokenService, TokenService>();
		services.AddAppDbContext(configuration);
		//services.AddSecurity(configuration);
		services.AddSecurity();
		//services.Configure<TokenOptions>(configuration.GetSection("Jwt"));
		services.AddOptions<TokenOptions>()
			.Bind(configuration.GetSection("Jwt"))
			.Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), "Jwt:Issuer missing")
			.Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "Jwt:Audience missing")
			.Validate(o => !string.IsNullOrWhiteSpace(o.SignInKey), "Jwt:SigninKey missing")
			.ValidateOnStart();
        services.AddCorsService();
        
        services.AddDocumentation();
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
    private static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi();
          services.AddSwaggerGen(options =>
          {
               var securityDefinition = new OpenApiSecurityScheme
               {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme=JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat= "JWT",
				In = ParameterLocation.Header,
                    Description="N'utiliser que le Token"
			};
			options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityDefinition);
			var securityRequirement = new OpenApiSecurityRequirement
			{
				{
                         new OpenApiSecurityScheme
                         {
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = JwtBearerDefaults.AuthenticationScheme
						}
					}, 
                         new string[] { }
                    }
			};
			options.AddSecurityRequirement(securityRequirement);
		});
        
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
    private static IServiceCollection AddSecurity(this IServiceCollection services)
	{
		static byte[] GetKeyBytes(TokenOptions o)
		{
			if (o.SignInKeyIsBase64)
				return Convert.FromBase64String(o.SignInKey.Trim());

			return Encoding.UTF8.GetBytes(o.SignInKey.Trim());
		}

		services
		    .AddAuthentication(options =>
		    {
			    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		    })
		    .AddJwtBearer(options =>
		    {
			    var serviceProvider = services.BuildServiceProvider();
			    var jwt = serviceProvider.GetRequiredService<IOptions<TokenOptions>>().Value;
			    var keyBytes = GetKeyBytes(jwt);
			    if (keyBytes.Length < 32)
				    throw new InvalidOperationException("Jwt:SigninKey too short (min 256 bits recommended).");

			    // Log non-sensible: length + hash (useful for diagnosing overrides)
			    var hash = Convert.ToHexString(SHA256.HashData(keyBytes))[..16];
			    Console.WriteLine($"[JWT] keyLen={keyBytes.Length} sha256={hash} issuer={jwt.Issuer} audience={jwt.Audience}");

			    options.RequireHttpsMetadata = !serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment(); // https in prod
			    options.SaveToken = true;
			    options.TokenValidationParameters = new TokenValidationParameters
			    {
				    ValidateIssuerSigningKey = true,
				    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
				    ValidateIssuer = true,
				    ValidIssuer = jwt.Issuer,
				    ValidateAudience = true,
				    ValidAudience = jwt.Audience,
				    ValidateLifetime = true,
				    ClockSkew = TimeSpan.FromMinutes(1) // strict but not punitive
			    };

			    options.Events = new JwtBearerEvents
			    {
				    OnMessageReceived = ctx =>
				  {
					  if (!string.IsNullOrEmpty(ctx.Token))
					  {
						  var jwtHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
						  var token = jwtHandler.ReadJwtToken(ctx.Token);
						  Console.WriteLine($"[JWT] token alg={token.Header.Alg} kid={token.Header.Kid ?? "(none)"}");
					  }
					  return Task.CompletedTask;
				  },
				    OnAuthenticationFailed = ctx =>
				  {
					  Console.WriteLine($"[JWT] FAILED {ctx.Exception.GetType().Name}: {ctx.Exception.Message}");
					  return Task.CompletedTask;
				  }
			    };
		    });

		services.AddAuthorization(options =>
		{
			// Example: a policy based on Role
			options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
		});

		
		return services;
	}

}
