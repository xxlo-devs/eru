using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUnknownUserMessageHandling(this IServiceCollection services) 
            => services.AddTransient<IUnkownUserMessageHandler, StartRegistrationMessageHandler>();
    }
}