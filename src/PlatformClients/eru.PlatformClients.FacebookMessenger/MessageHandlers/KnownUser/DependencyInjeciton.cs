using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser
{
    public static class DependencyInjeciton
    {
        public static IServiceCollection AddKnownUserMessageHandling(this IServiceCollection services)
        {
            services.AddTransient<ICancelSubscriptionMessageHandler, CancelSubscriptionMessageHandler>();
            services.AddTransient<IUnsupportedCommandMessageHandler, UnsupportedCommandMessageHandler>();
            services.AddTransient<IKnownUserMessageHandler, KnownUserMessageHandler>();
            
            return services;
        }
    }
}