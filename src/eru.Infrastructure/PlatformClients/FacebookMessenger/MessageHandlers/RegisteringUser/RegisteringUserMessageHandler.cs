using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.UnsupportedCommand;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.Paging;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using Microsoft.AspNetCore.Routing.Template;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public class RegisteringUserMessageHandler : IRegisteringUserMessageHandler
    {
        private readonly ICancelRegistrationMessageHandler _cancelHandler;
        private readonly IConfirmSubscriptionMessageHandler _confirmHandler;
        private readonly IGatherClassMessageHandler _classHandler;
        private readonly IGatherLanguageMessageHandler _langHandler;
        private readonly IGatherYearMessageHandler _yearHandler;
        private readonly IUnsupportedCommandMessageHandler _unsupportedHandler;
        private readonly IPagingMessageHandler _pagingHandler;

        public RegisteringUserMessageHandler(ICancelRegistrationMessageHandler cancelHandler, IConfirmSubscriptionMessageHandler confirmHandler, IGatherClassMessageHandler classHandler, IGatherLanguageMessageHandler langHandler, IGatherYearMessageHandler yearHandler, IUnsupportedCommandMessageHandler unsupportedHandler, IPagingMessageHandler pagingHandler)
        {
            _cancelHandler = cancelHandler;
            _confirmHandler = confirmHandler;
            _classHandler = classHandler;
            _langHandler = langHandler;
            _yearHandler = yearHandler;
            _unsupportedHandler = unsupportedHandler;
            _pagingHandler = pagingHandler;
        }
        public async Task Handle(string uid, Message message)
        {
            if (message?.QuickReply?.Payload != null)
            {
                switch (message.QuickReply.Payload)
                {
                    case ReplyPayloads.CancelPayload:
                        await _cancelHandler.Handle(uid);
                        break;
                    
                    case ReplyPayloads.SubscribePayload:
                        await _confirmHandler.Handle(uid);
                        break;
                    
                    case ReplyPayloads.PreviousPage:
                        await _pagingHandler.ShowPreviousPage(uid);
                        break;
                    
                    case ReplyPayloads.NextPage:
                        await _pagingHandler.ShowNextPage(uid);
                        break;
                }

                if (message.QuickReply.Payload.StartsWith(ReplyPayloads.LangPrefix))
                {
                    await _langHandler.Handle(uid, message.QuickReply.Payload);
                }
                
                if (message.QuickReply.Payload.StartsWith(ReplyPayloads.YearPrefix))
                {
                    await _yearHandler.Handle(uid, message.QuickReply.Payload);
                }

                if (message.QuickReply.Payload.StartsWith(ReplyPayloads.ClassPrefix))
                {
                    await _classHandler.Handle(uid, message.QuickReply.Payload);
                }
            }
            else
            {
                await _unsupportedHandler.Handle(uid);
            }
        }
    }
}