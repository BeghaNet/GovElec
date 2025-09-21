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
using FluentValidation;
using GovElec.Api.Validations.Users;
using GovElec.Api.Validations.Demands;
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
		services.AddValidation();
		services.AddSecurity();
		services.AddOptions<TokenOptions>()
			.Bind(configuration.GetSection("Jwt"))
			.Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), "Jwt:Issuer missing")
			.Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "Jwt:Audience missing")
			.Validate(o => !string.IsNullOrWhiteSpace(o.SignInKey), "Jwt:SigninKey missing")
			.ValidateOnStart();
		services.AddCorsService();
		services.AddDocumentation();
		services.AddScoped<IPasswordService, PasswordService>();
		ConfigureMapster();

		//Devra disparaitre : Ne sert que pour les tests
		services.AddScoped<IPersonService, PersonService>();
		return services;
	}

	/// <summary>
	/// Recherches et ajoute les Endpoints dans la collection de services.
	/// </summary>
	/// <param name="services">La collection de service à laquelle ajouter les endpoints.</param>
	/// <param name="assembly">L'assembly qui contient les endpoints à ajouter.</param>
	/// <returns>La collection de service mise à jour.</returns>
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
	/// <summary>
	/// Ajoute la documentation Swagger / OpenApi
	/// </summary>
	/// <param name="services"></param>
	/// <returns>La collection de services mise à jours.</returns>
	private static IServiceCollection AddDocumentation(this IServiceCollection services)
	{
		services.AddOpenApi();
		services.AddSwaggerGen(options =>
		{
			var securityDefinition = new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = JwtBearerDefaults.AuthenticationScheme,
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "N'utiliser que le Token"
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
	/// <summary>
	/// Ajoute le DbContext de l'application
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configuration"></param>
	/// <returns></returns>
	private static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("SQLiteConnection")));
		return services;
	}

	/// <summary>
	/// Ajoute le service CORS
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
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
	/// <summary>
	/// Ajoute la sécurité (Authentication + Authorization)
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
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
			options.AddPolicy("AdminOnly", p => p.RequireRole("Admin", "SuperAdmin"));
			options.AddPolicy("SuperAdminOnly", p => p.RequireRole("SuperAdmin"));
			options.AddPolicy("UserOnly", p => p.RequireRole("User", "Admin", "SuperAdmin"));
			options.AddPolicy("ButOnly", p => p.RequireRole("But", "Admin", "SuperAdmin"));
			options.AddPolicy("AnyLoggedUser", p => p.RequireAuthenticatedUser());
			options.AddPolicy("SelfOrAdmin", policy =>
	   policy.RequireAuthenticatedUser()
			.RequireAssertion(ctx =>
			{
				var user = ctx.User;

				// Admin ? → OK
				if (user.IsInRole("Admin")) return true;
				if (user.IsInRole("SuperAdmin")) return true;
				// Comparer l'id de la route avec l'id du user
				if (ctx.Resource is HttpContext http)
				{
					// nom du paramètre de route : ici "id"
					var routeId = http.Request.RouteValues["username"]?.ToString();

					// quel claim contient l'identité "métier" ?
					var userId =
					   user.FindFirst(ClaimTypes.Name)?.Value;       // ← chez toi, c'est souvent Name

					return !string.IsNullOrEmpty(routeId)
						 && !string.IsNullOrEmpty(userId)
						 && string.Equals(routeId, userId, StringComparison.OrdinalIgnoreCase);
				}
				return false;
			}));
		});


		return services;
	}
	/// <summary>
	/// Ajoute les validateurs FluentValidation
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	private static IServiceCollection AddValidation(this IServiceCollection services)
	{
		services.AddScoped<IValidator<UserForCreateCommand>, CreateUserValidator>();
		services.AddScoped<IValidator<UserForUpdateCommand>, UpdateUserValidator>();
		services.AddScoped<IValidator<ChangePasswordCommand>, ChangePasswordValidator>();
		services.AddScoped<IValidator<DemandForCreateCommand>, CreateDemandValidator>();
		services.AddScoped<IValidator<DemandForUpdateCommand>, UpdateDemandValidator>();
		return services;
	}
	/// <summary>
	/// Configure Mapster pour les conversions spécifiques
	/// </summary>
	private static void ConfigureMapster()
	{
		TypeAdapterConfig<DateTime, DateOnly>
		    .NewConfig()
		    .MapWith(src => DateOnly.FromDateTime(src));

		TypeAdapterConfig<DateTime?, DateOnly?>
		    .NewConfig()
		    .MapWith(src => src.HasValue ? DateOnly.FromDateTime(src.Value) : (DateOnly?)null);

		// (optionnel, sens inverse)
		TypeAdapterConfig<DateOnly, DateTime>
		    .NewConfig()
		    .MapWith(src => src.ToDateTime(TimeOnly.MinValue));

		TypeAdapterConfig<DateOnly?, DateTime?>
		    .NewConfig()
		    .MapWith(src => src.HasValue ? src.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null);
	}

}
