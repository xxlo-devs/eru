using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.CancelSubscriptionMessageHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.ConfirmSubscriptionMessageHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.GatherClassMessageHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.GatherLanguageMessageHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.GatherYearMessageHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.UnsupportedCommandMessageHandler;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRegisteringUserHandling(this IServiceCollection services)
        {
            services.AddTransient<IRegisteringUserMessageHandler, RegisteringUserMessageHandler>();
            services.AddTransient<IUnsupportedCommandMessageHandler, UnsupportedCommandMessageHandler.UnsupportedCommandMessageHandler>();
            services.AddTransient<ICancelSubscriptionMessageHandler, CancelSubsciptionMessageHandler>();
            services.AddTransient<IGatherLanguageMessageHandler, GatherLanguageMessageHandler.GatherLanguageMessageHandler>();
            services.AddTransient<IGatherYearMessageHandler, GatherYearMessageHandler.GatherYearMessageHandler>();
            services.AddTransient<IGatherClassMessageHandler, GatherClassMessageHandler.GatherClassMessageHandler>();
            services.AddTransient<IConfirmSubscriptionMessageHandler, ConfirmSubscriptionMessageHandler.ConfirmSubscriptionMessageHandler>();
            return services;
        }
    }
}