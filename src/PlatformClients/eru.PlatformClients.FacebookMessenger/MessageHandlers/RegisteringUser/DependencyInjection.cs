using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear;
using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRegisteringUserMessageHandling(this IServiceCollection services)
        {
            services.AddTransient<RegistrationMessageHandler<CancelRegistrationMessageHandler>, CancelRegistrationMessageHandler>();
            services.AddTransient<RegistrationMessageHandler<ConfirmSubscriptionMessageHandler>, ConfirmSubscriptionMessageHandler>();
            services.AddTransient<RegistrationMessageHandler<GatherClassMessageHandler>, GatherClassMessageHandler>();
            services.AddTransient<RegistrationMessageHandler<GatherLanguageMessageHandler>, GatherLanguageMessageHandler>();
            services.AddTransient<RegistrationMessageHandler<GatherYearMessageHandler>, GatherYearMessageHandler>();
            services.AddTransient<MessageHandler<RegisteringUserMessageHandler>, RegisteringUserMessageHandler>();

            return services;
        }
    }
}