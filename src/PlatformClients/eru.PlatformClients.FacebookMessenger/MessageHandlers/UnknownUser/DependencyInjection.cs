using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUnknownUserMessageHandling(this IServiceCollection services) 
            => services.AddTransient<IUnknownUserMessageHandler, UnknownUserMessageHandler>();
    }
}