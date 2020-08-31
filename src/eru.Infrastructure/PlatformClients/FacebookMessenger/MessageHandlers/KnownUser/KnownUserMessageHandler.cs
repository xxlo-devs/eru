﻿using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser
{
    public class KnownUserMessageHandler : IKnownUserMessageHandler
    {
        private readonly ICancelSubscriptionMessageHandler _cancelSubscriptionMessageHandler;
        private readonly IUnsupportedCommandMessageHandler _unsupportedCommandMessageHandler;

        public KnownUserMessageHandler(ICancelSubscriptionMessageHandler cancelSubscriptionMessageHandler, IUnsupportedCommandMessageHandler unsupportedCommandMessageHandler)
        {
            _cancelSubscriptionMessageHandler = cancelSubscriptionMessageHandler;
            _unsupportedCommandMessageHandler = unsupportedCommandMessageHandler;
        }
        
        public async Task Handle(string uid, Message message)
        {
            if (message?.QuickReply?.Payload == ReplyPayloads.CancelPayload)
            {
                await _cancelSubscriptionMessageHandler.Handle(uid);
                return;
            }

            await _unsupportedCommandMessageHandler.Handle(uid);
        }
    }
}