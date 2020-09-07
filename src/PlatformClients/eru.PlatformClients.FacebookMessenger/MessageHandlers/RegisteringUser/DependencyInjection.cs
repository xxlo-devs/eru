using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps;
using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRegisteringUserMessageHandling(this IServiceCollection services)
        {
            services.AddTransient<MessageHandler<CancelRegistrationMessageHandler>, CancelRegistrationMessageHandler>();
            services.AddTransient<RegistrationEndMessageHandler<ConfirmSubscriptionMessageHandler>, ConfirmSubscriptionMessageHandler>();
            services.AddTransient<RegistrationStepsMessageHandler<GatherClassMessageHandler>, GatherClassMessageHandler>();
            services.AddTransient<RegistrationStepsMessageHandler<GatherLanguageMessageHandler>, GatherLanguageMessageHandler>();
            services.AddTransient<RegistrationStepsMessageHandler<GatherYearMessageHandler>, GatherYearMessageHandler>();
            services.AddTransient<MessageHandler<RegisteringUserMessageHandler>, RegisteringUserMessageHandler>();

            return services;
        }
    }
}