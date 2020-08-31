using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.UnsupportedCommand;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRegisteringUserMessageHandling(this IServiceCollection services)
        {
            services.AddTransient<ICancelRegistrationMessageHandler, CancelRegistrationMessageHandler>();
            services.AddTransient<IConfirmSubscriptionMessageHandler, ConfirmSubscriptionMessageHandler>();
            services.AddTransient<IGatherClassMessageHandler, GatherClassMessageHandler>();
            services.AddTransient<IGatherLanguageMessageHandler, GatherLanguageMessageHandler>();
            services.AddTransient<IGathterYearMessageHandler, GatherYearMessageHandler>();
            services.AddTransient<IUnsupportedCommandMessageHandler, UnsupportedCommandMessageHandler>();
            services.AddTransient<IRegisteringUserMessageHandler, RegisteringUserMessageHandler>();

            return services;
        }
    }
}