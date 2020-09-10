using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser
{
    public static class DependencyInjeciton
    {
        public static IServiceCollection AddKnownUserMessageHandling(this IServiceCollection services)
        {
            services.AddTransient<MessageHandler<CancelSubscriptionMessageHandler>, CancelSubscriptionMessageHandler>();
            services.AddTransient<MessageHandler<UnsupportedCommandMessageHandler>, UnsupportedCommandMessageHandler>();
            services.AddTransient<MessageHandler<KnownUserMessageMessageHandler>, KnownUserMessageMessageHandler>();
            
            return services;
        }
    }
}