using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser
{
    public class KnownUserMessageHandler : IKnownUserMessageHandler
    {
        private readonly ICancelSubscriptionMessageHandler _cancelSubscriptionMessageHandler;
        private readonly IUnsupportedCommandMessageHandler _unsupportedCommandMessageHandler;
        private readonly ILogger _logger;

        public KnownUserMessageHandler(ICancelSubscriptionMessageHandler cancelSubscriptionMessageHandler, IUnsupportedCommandMessageHandler unsupportedCommandMessageHandler, ILogger logger)
        {
            _cancelSubscriptionMessageHandler = cancelSubscriptionMessageHandler;
            _unsupportedCommandMessageHandler = unsupportedCommandMessageHandler;
            _logger = logger;
        }
        
        public async Task Handle(string uid, Message message)
        {
            _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: KnownUserMessageHandler.Handle got a request (uid: {uid}, payload: {message?.QuickReply?.Payload})");
            
            if (message?.QuickReply?.Payload != null)
            {
                var payload = JsonSerializer.Deserialize<Payload>(message.QuickReply.Payload,
                    new JsonSerializerOptions
                    {
                        IgnoreNullValues = true,
                        Converters = {new JsonStringEnumConverter()}
                    });
                
                _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: KnownUserMessageHandler.Handle deserialized payload; Type: {payload.Type}, Id: {payload.Id}, Page: {payload.Page}");

                if (payload?.Type == PayloadType.Cancel)
                {
                    _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: KnownUserMessageHandler.Handle redirected request to CancelSubscriptionMessageHandler.Handle");
                    await _cancelSubscriptionMessageHandler.Handle(uid);
                    return;
                }
            }

            _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: KnownUserMessageHandler.Handle redirected request to UnsupportedCommandMessageHandler.Handle");
            await _unsupportedCommandMessageHandler.Handle(uid);
        }
    }
}