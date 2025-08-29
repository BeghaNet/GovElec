

using System;
namespace GovElec.App.Components;

public static class ServicesExtension
{
    public static IServiceCollection AddServicesExtension(this IServiceCollection services, IConfiguration configuration)
    {
        // Add any additional services here
        // For example, you can add a HttpClient or other services as needed
        // services
        {
            services.AddRazorComponents()
                .AddInteractiveServerComponents();
            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!) });

            return services;
        }
    }
}
