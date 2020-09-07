using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser
{
    public class KnownUserMessageHandler : MessageHandler<KnownUserMessageHandler>
    {
        private readonly MessageHandler<CancelSubscriptionMessageHandler> _cancelHandler;
        private readonly MessageHandler<UnsupportedCommandMessageHandler> _unsupportedHandler;
        
        public KnownUserMessageHandler(MessageHandler<CancelSubscriptionMessageHandler> cancelHandler, MessageHandler<UnsupportedCommandMessageHandler> unsupportedHandler, ILogger<KnownUserMessageHandler> logger) : base(logger)
        {
            _cancelHandler = cancelHandler;
            _unsupportedHandler = unsupportedHandler;
        }

        protected override async Task Base(Messaging message)
        {
            if (message?.Message?.QuickReply?.Payload != null)
            {
                var payload = JsonSerializer.Deserialize<Payload>(message?.Message?.QuickReply?.Payload,
                    new JsonSerializerOptions
                    {
                        IgnoreNullValues = true,
                        Converters = {new JsonStringEnumConverter()}
                    });
                

                if (payload?.Type == PayloadType.Cancel)
                {
                    await _cancelHandler.Handle(message);
                    return;
                }
            }

            await _unsupportedHandler.Handle(message);
        }
    }
}