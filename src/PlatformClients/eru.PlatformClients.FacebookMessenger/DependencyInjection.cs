using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.RegistrationDb;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFacebookMessenger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFacebookMessengerRegistrationDatabase();
            services.AddMessageHandling();
            services.AddSendApiClient();
            services.AddSelector();
            services.AddTransient<FbMiddleware>();
            services.AddTransient<IPlatformClient, FacebookMessengerPlatformClient>();
            
            return services;
        }

        public static IApplicationBuilder UseFacebookMessenger(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.Map("/fbwebhook", x => x.UseMiddleware<FbMiddleware>());
            
            return app;
        }
    }
}