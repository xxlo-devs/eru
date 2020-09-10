﻿using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser
{
    public interface IKnownUserMessageHandler
    {
        public Task Handle(Messaging message);
    }
}