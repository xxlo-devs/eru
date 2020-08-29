using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserMessageHandler.CancelSubscriptionHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.UnsupportedCommandMessageHandler;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserMessageHandler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddKnownUserMessageHandling(this IServiceCollection services)
        {
            services.AddTransient<IKnownUserMessageHandler, KnownUserMessageHandler>();
            services.AddTransient<IUnsupportedCommandMessageHandler, UnsupportedCommandMessageHandler>();
            services.AddTransient<ICancelSubscriptionMessageHandler, CancelSubscriptionMessageHandler>();
            return services;
        }
    }
}