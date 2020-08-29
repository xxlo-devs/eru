using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUserMessageHandler.CreateUserMessageHandler;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUserMessageHandler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUnknownUserMessageHandling(this IServiceCollection services)
        {
            services.AddTransient<IUnknownUserMessageHandler, UnknownUserMessageHandler>();
            services.AddTransient<ICreateUserMessageHandler, CreateUserMessageHandler.CreateUserMessageHandler>();
            return services;
        }
    }
}