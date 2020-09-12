using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.Middleware;
using eru.PlatformClients.FacebookMessenger.RegistrationDb;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFacebookMessenger(this IServiceCollection services)
        {
            services.AddFacebookMessengerRegistrationDatabase();
            services.AddSendApiClient();
            services.AddMessageHandling();
            services.AddFbMiddleware();
            services.AddTransient<IPlatformClient, FacebookMessengerPlatformClient>();
            
            return services;
        }

        public static IApplicationBuilder UseFacebookMessenger(this IApplicationBuilder app)
        {
            app.UseFbMiddleware();
            
            return app;
        }
    }
}