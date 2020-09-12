using System;
using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.SendAPIClient
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSendApiClient(this IServiceCollection services)
        {
            const string graphEndpoint = "https://graph.facebook.com/v8.0/me/messages";
            services.AddHttpClient(FacebookMessengerPlatformClient.PId, c =>
            {
                c.BaseAddress = new Uri(graphEndpoint);
            });
            
            services.AddTransient<ISendApiClient, SendApiClient>();

            return services;
        }
    }
}