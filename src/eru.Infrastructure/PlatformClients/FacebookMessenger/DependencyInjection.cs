using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFacebookMessenger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFacebookMessengerRegistrationDatabase();
            services.AddMessageHandling();
            services.AddTransient<FbMiddleware>();
            
            return services;
        }

        public static IApplicationBuilder UseFacebookMessenger(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.Map("/fbwebhook", x => x.UseMiddleware<FbMiddleware>());
            
            return app;
        }
    }
}