using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace GovElec.App.Extensions;

public static class ServicesExtension
{
	public static IServiceCollection AddServicesExtension(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddRazorComponents()
			   .AddInteractiveServerComponents();
		services.AddHttpContextAccessor();
		services.ConfigureHttpClient(configuration);
		services.AddAuthenticationCore();
		services.AddScoped<CookieService>();
		services.AddScoped<TokenService>();
		services.AddScoped<CustomAuthenticationProvider>();
		services.AddScoped<AuthenticationStateProvider, CustomAuthenticationProvider>();
		services.AddScoped<AuthService>();
		services.ConfigureAutentication(configuration);
		services.AddCascadingAuthenticationState();
		services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!) });

		return services;


	}
	private static IServiceCollection ConfigureHttpClient(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddHttpContextAccessor();
		services.AddTransient<JwtCookieHandler>();
		services.AddHttpClient("GovElecApi", (sp, client) =>
		{
			var timeout = int.Parse(configuration["ApiSettings:TimeoutSeconds"]!);
			var retryCount = int.Parse(configuration["ApiSettings:RetryCount"]!);
			var accessor = sp.GetRequiredService<IHttpContextAccessor>();
			var req = accessor.HttpContext!.Request;
			
			client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!);
			client.Timeout = TimeSpan.FromSeconds(timeout); // Example: Set a timeout
		}).AddHttpMessageHandler<JwtCookieHandler>();
		
		// services.AddHttpClient("GovElecApi", client =>
		// {
		//     var timeout = int.Parse(configuration["ApiSettings:TimeoutSeconds"]!);
		//     var retryCount = int.Parse(configuration["ApiSettings:RetryCount"]!);
		//     client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!);
		//     client.Timeout = TimeSpan.FromSeconds(timeout); // Example: Set a timeout

		// });
		return services;
	}
	
	private static IServiceCollection ConfigureAutentication(this IServiceCollection services,IConfiguration configuration)
	{
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		    .AddJwtBearer(options =>
		    {
			    options.TokenValidationParameters = new TokenValidationParameters
			    {
				    ValidateIssuer = true,
				    ValidateAudience = true,
				    ValidateLifetime = true,
				    ValidateIssuerSigningKey = true,

				    // D'apr�s ton JWT : iss = "GovElecApi", aud = "GovElecApplication"
				    ValidIssuer = configuration["Jwt:Issuer"],
				    ValidAudience = configuration["Jwt:Audience"],
				    IssuerSigningKey = new SymmetricSecurityKey(
					  Encoding.UTF8.GetBytes(configuration["Jwt:SignInKey"]!)),

				    // Pour les claims Name/Role tels que pr�sents dans ton token
				    NameClaimType = ClaimTypes.Name,   // http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name
				    RoleClaimType = ClaimTypes.Role    // http://schemas.microsoft.com/ws/2008/06/identity/claims/role
			    };

			    options.Events = new JwtBearerEvents
			    {
				    OnMessageReceived = ctx =>
				    {
					    if (ctx.Request.Cookies.TryGetValue("token", out var cookieToken))
						    ctx.Token = cookieToken;
					    return Task.CompletedTask;
				    },
				    // (facultatif) utile pour comprendre un 401
				    OnAuthenticationFailed = ctx =>
				    {
					    ctx.NoResult();
					    return Task.CompletedTask;
				    }
			    };
			    options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
		    });

		services.AddAuthorization();
		return services;
	}
	
}
