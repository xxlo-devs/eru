using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser
{
    public class KnownUserMessageMessageHandler : MessageHandler<KnownUserMessageMessageHandler>, IKnownUserMessageHandler
    {
        private readonly ICancelSubscriptionMessageHandler _cancelHandler;
        private readonly IUnsupportedCommandMessageHandler _unsupportedHandler;
        
        public KnownUserMessageMessageHandler(ICancelSubscriptionMessageHandler cancelHandler, IUnsupportedCommandMessageHandler unsupportedHandler, ILogger<KnownUserMessageMessageHandler> logger) : base(logger)
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