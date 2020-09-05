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

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public class RegisteringUserMessageHandler : IRegisteringUserMessageHandler
    {
        private readonly ICancelRegistrationMessageHandler _cancelHandler;
        private readonly IConfirmSubscriptionMessageHandler _confirmHandler;
        private readonly IGatherClassMessageHandler _classHandler;
        private readonly IGatherLanguageMessageHandler _langHandler;
        private readonly IGatherYearMessageHandler _yearHandler;
        private readonly IRegistrationDbContext _dbContext;

        public RegisteringUserMessageHandler(IRegistrationDbContext dbContext, ICancelRegistrationMessageHandler cancelHandler, IConfirmSubscriptionMessageHandler confirmHandler, IGatherClassMessageHandler classHandler, IGatherLanguageMessageHandler langHandler, IGatherYearMessageHandler yearHandler)
        {
            _dbContext = dbContext;
            _cancelHandler = cancelHandler;
            _confirmHandler = confirmHandler;
            _classHandler = classHandler;
            _langHandler = langHandler;
            _yearHandler = yearHandler;
        }
        public async Task Handle(string uid, Message message)
        {
            var payload = new Payload();
            
            if (message?.QuickReply?.Payload != null)
            {
                payload = JsonSerializer.Deserialize<Payload>(message.QuickReply.Payload, new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    Converters = { new JsonStringEnumConverter() }
                });
                
                if (payload.Type == PayloadType.Cancel)
                {
                    await _cancelHandler.Handle(uid);
                    return;
                }
            }

            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            switch (user.Stage)
            {
                case Stage.Created:
                {
                    await _langHandler.Handle(uid, payload);
                    break;
                }

                case Stage.GatheredLanguage:
                {
                    await _yearHandler.Handle(uid, payload);
                    break;
                }

                case Stage.GatheredYear:
                {
                    await _classHandler.Handle(uid, payload);
                    break;
                }

                case Stage.GatheredClass:
                {
                    await _confirmHandler.Handle(uid, payload);
                    break;
                }
            }
        }
    }
}