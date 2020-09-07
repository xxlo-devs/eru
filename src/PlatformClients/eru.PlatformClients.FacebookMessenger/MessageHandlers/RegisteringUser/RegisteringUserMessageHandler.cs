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
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public class RegisteringUserMessageHandler : MessageHandler<RegisteringUserMessageHandler>
    {
        /*private readonly ICancelRegistrationMessageHandler _cancelHandler;
        private readonly IConfirmSubscriptionMessageHandler _confirmHandler;
        private readonly IGatherClassMessageHandler _classHandler;
        private readonly IGatherLanguageMessageHandler _langHandler;
        private readonly IGatherYearMessageHandler _yearHandler;
        private readonly IRegistrationDbContext _dbContext;
        private readonly ILogger<RegisteringUserMessageHandler> _logger;

        public RegisteringUserMessageHandler(IRegistrationDbContext dbContext, ICancelRegistrationMessageHandler cancelHandler, IConfirmSubscriptionMessageHandler confirmHandler, IGatherClassMessageHandler classHandler, IGatherLanguageMessageHandler langHandler, IGatherYearMessageHandler yearHandler, ILogger<RegisteringUserMessageHandler> logger)
        {
            _dbContext = dbContext;
            _cancelHandler = cancelHandler;
            _confirmHandler = confirmHandler;
            _classHandler = classHandler;
            _langHandler = langHandler;
            _yearHandler = yearHandler;
            _logger = logger;
        }
        public async Task Handle(string uid, Message message)
        {
            _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: RegisteringUserMessageHandler.Handle got a message (uid: {uid}, payload: {message?.QuickReply?.Payload})");
            var payload = new Payload();
            
            if (message?.QuickReply?.Payload != null)
            {
                payload = JsonSerializer.Deserialize<Payload>(message.QuickReply.Payload, new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    Converters = { new JsonStringEnumConverter() }
                });
                
                _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: RegisteringUserMessageHandler.Handle has deserialized payload; Type: {payload.Type}, Id: {payload.Id}, Page: {payload.Page}");
                
                if (payload.Type == PayloadType.Cancel)
                {
                    _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: RegisteringUserMessageHandler.Handle redirected request to CancelRegistrationMessageHandler.Handle");
                    await _cancelHandler.Handle(uid);
                    return;
                }
            }

            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            switch (user.Stage)
            {
                case Stage.Created:
                {
                    _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: RegisteringUserMessageHandler.Handle redirected request to GatherLanguageMessageHandler");
                    await _langHandler.Handle(uid, payload);
                    break;
                }

                case Stage.GatheredLanguage:
                {
                    _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: RegisteringUserMessageHandler.Handle redirected request to GatherYearMessageHandler");
                    await _yearHandler.Handle(uid, payload);
                    break;
                }

                case Stage.GatheredYear:
                {
                    _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: RegisteringUserMessageHandler.Handle redirected request to GatherClassMessageHandler");
                    await _classHandler.Handle(uid, payload);
                    break;
                }

                case Stage.GatheredClass:
                {
                    _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: RegisteringUserMessageHandler.Handle redirected request to ConfirmSubscriptionMessageHandler");
                    await _confirmHandler.Handle(uid, payload);
                    break;
                }
            }
        }*/
        public RegisteringUserMessageHandler(IServiceProvider provider, ILogger<RegisteringUserMessageHandler> logger) : base(logger)
        {
        }

        protected override async Task Base(Messaging message)
        {
            throw new System.NotImplementedException();
        }
    }
}