using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessageHandling(this IServiceCollection services)
        {
            services.AddKnownUserMessageHandling();
            services.AddRegisteringUserMessageHandling();
            services.AddUnknownUserMessageHandling();
            services.AddTransient<IMessageHandler, IncomingMessageHandler>();
            return services;
        }
    }
}