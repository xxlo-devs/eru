using System.Text.Json;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
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
            var payload = JsonSerializer.Deserialize<Payload>(message.QuickReply.Payload);
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);

            if (payload.Type == Type.Cancel)
            {
                await _cancelHandler.Handle(uid);
                return;
            }
            
            switch (user.Stage)
            {
                case Stage.Created:
                {
                    await _langHandler.Handle(uid, message.QuickReply.Payload);
                    break;
                }

                case Stage.GatheredLanguage:
                {
                    await _yearHandler.Handle(uid, message.QuickReply.Payload);
                    break;
                }

                case Stage.GatheredYear:
                {
                    await _classHandler.Handle(uid, message.QuickReply.Payload);
                    break;
                }

                case Stage.GatheredClass:
                {
                    await _confirmHandler.Handle(uid, message.QuickReply.Payload);
                    break;
                }
            }
        }
    }
}