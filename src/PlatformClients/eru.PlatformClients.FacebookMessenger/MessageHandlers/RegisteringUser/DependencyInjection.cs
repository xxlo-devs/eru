using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.CancelRegistration;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear;
using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRegisteringUserMessageHandling(this IServiceCollection services)
        {
            services.AddTransient<ICancelRegistrationMessageHandler, CancelRegistrationMessageHandler>();
            services.AddTransient<IConfirmSubscriptionMessageHandler, ConfirmSubscriptionMessageHandler>();
            services.AddTransient<IGatherClassMessageHandler, GatherClassMessageHandler>();
            services.AddTransient<IGatherLanguageMessageHandler, GatherLanguageMessageHandler>();
            services.AddTransient<IGatherYearMessageHandler, GatherYearMessageHandler>();
            services.AddTransient<IRegisteringUserMessageHandler, RegisteringUserMessageHandler>();

            return services;
        }
    }
}