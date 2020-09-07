using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser
{
    public static class DependencyInjeciton
    {
        public static IServiceCollection AddKnownUserMessageHandling(this IServiceCollection services)
        {
            services.AddTransient<MessageHandler<CancelSubscriptionMessageHandler>, CancelSubscriptionMessageHandler>();
            services.AddTransient<MessageHandler<UnsupportedCommandMessageHandler>, UnsupportedCommandMessageHandler>();
            services.AddTransient<MessageHandler<KnownUserMessageHandler>, KnownUserMessageHandler>();
            
            return services;
        }
    }
}