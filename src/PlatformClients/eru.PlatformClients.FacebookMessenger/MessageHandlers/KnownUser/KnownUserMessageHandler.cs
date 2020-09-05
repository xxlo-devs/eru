using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser
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
            if (message?.QuickReply?.Payload != null)
            {
                var payload = JsonSerializer.Deserialize<Payload>(message.QuickReply.Payload,
                    new JsonSerializerOptions
                    {
                        IgnoreNullValues = true,
                        Converters = {new JsonStringEnumConverter()}
                    });

                if (payload?.Type == PayloadType.Cancel)
                {
                    await _cancelSubscriptionMessageHandler.Handle(uid);
                    return;
                }
            }

            await _unsupportedCommandMessageHandler.Handle(uid);
        }
    }
}