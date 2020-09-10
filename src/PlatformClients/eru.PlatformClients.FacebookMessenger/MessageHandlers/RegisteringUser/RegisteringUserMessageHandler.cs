using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.CancelRegistration;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public class RegisteringUserMessageHandler : MessageHandler<RegisteringUserMessageHandler>, IRegisteringUserMessageHandler
    {
        private readonly IServiceProvider _provider;

        public RegisteringUserMessageHandler(IServiceProvider provider, ILogger<RegisteringUserMessageHandler> logger) : base(logger)
        {
            _provider = provider;
        }

        protected override async Task Base(Messaging message)
        {
            var user = await _provider.GetService<IRegistrationDbContext>().IncompleteUsers.FindAsync(message.Sender.Id);
            var payload = new Payload();
            
            if (message.Message?.QuickReply?.Payload != null)
            {
                payload = JsonSerializer.Deserialize<Payload>(message.Message?.QuickReply?.Payload, new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    Converters = { new JsonStringEnumConverter() }
                });
                
                if (payload.Type == PayloadType.Cancel)
                {
                    await _provider.GetService<MessageHandler<CancelRegistrationMessageHandler>>().Handle(message);
                    return;
                }
            }
            
            switch (user.Stage)
            {
                case Stage.Created:
                {
                    await _provider.GetService<RegistrationStepsMessageHandler<GatherLanguageMessageHandler>>().Handle(user, payload);
                    break;
                }

                case Stage.GatheredLanguage:
                {
                    await _provider.GetService<RegistrationStepsMessageHandler<GatherYearMessageHandler>>().Handle(user, payload);
                    break;
                }

                case Stage.GatheredYear:
                {
                    await _provider.GetService<RegistrationStepsMessageHandler<GatherClassMessageHandler>>().Handle(user, payload);
                    break;
                }

                case Stage.GatheredClass:
                {
                    await _provider.GetService<RegistrationEndMessageHandler<ConfirmSubscriptionMessageHandler>>().Handle(message);
                    break;
                }
            }
        }
    }
}