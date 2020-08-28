using eru.Infrastructure.PlatformClients.FacebookMessenger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.PlatformClients
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPlatformClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFacebookMessenger(configuration);
            return services;
        }

        public static IApplicationBuilder UsePlatformClients(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseFacebookMessenger(configuration);
            return app;
        }
    }
}