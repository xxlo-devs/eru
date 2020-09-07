using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public class RegisteringUserMessageHandler : MessageHandler<RegisteringUserMessageHandler>
    {
        private readonly IServiceProvider _provider;

        public RegisteringUserMessageHandler(IServiceProvider provider, ILogger<RegisteringUserMessageHandler> logger) : base(logger)
        {
            _provider = provider;
        }

        protected override async Task Base(Messaging message)
        {
            var payload = new Payload();
            
            if (message.Message.QuickReply.Payload != null)
            {
                payload = JsonSerializer.Deserialize<Payload>(message.Message.QuickReply.Payload, new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    Converters = { new JsonStringEnumConverter() }
                });
                
                if (payload.Type == PayloadType.Cancel)
                {
                    await _provider.GetService<RegistrationMessageHandler<CancelRegistrationMessageHandler>>().Handle(message.Sender.Id, payload);
                    return;
                }
            }

            var user = await _provider.GetService<IRegistrationDbContext>().IncompleteUsers.FindAsync(message.Sender.Id, payload);
            
            switch (user.Stage)
            {
                case Stage.Created:
                {
                    await _provider.GetService<RegistrationMessageHandler<GatherLanguageMessageHandler>>().Handle(message.Sender.Id, payload);
                    break;
                }

                case Stage.GatheredLanguage:
                {
                    await _provider.GetService<RegistrationMessageHandler<GatherYearMessageHandler>>().Handle(message.Sender.Id, payload);
                    break;
                }

                case Stage.GatheredYear:
                {
                    await _provider.GetService<RegistrationMessageHandler<GatherClassMessageHandler>>().Handle(message.Sender.Id, payload);
                    break;
                }

                case Stage.GatheredClass:
                {
                    await _provider.GetService<RegistrationMessageHandler<ConfirmSubscriptionMessageHandler>>().Handle(message.Sender.Id, payload);
                    break;
                }
            }
        }
    }
}