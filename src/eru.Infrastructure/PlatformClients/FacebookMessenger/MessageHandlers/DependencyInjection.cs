using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserMessageHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUserMessageHandler;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessageHandling(this IServiceCollection services)
        {
            services.AddKnownUserMessageHandling();
            services.AddRegisteringUserHandling();
            services.AddUnknownUserMessageHandling();
            services.AddTransient<IMessageHandler, IncomingMessageHandler>();
            return services;
        }
    }
}