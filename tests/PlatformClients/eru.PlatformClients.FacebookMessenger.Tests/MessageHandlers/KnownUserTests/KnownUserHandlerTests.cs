using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.KnownUserTests
{
    public class KnownUserHandlerTests
    {
        [Fact]
        public async void ShouldRouteCancelSubscriptionRequestToCancelSubscriptionHandler()
        {
            var cancelSubscriptionHandler = new Mock<ICancelSubscriptionMessageHandler>();
            var unsupportedCommandHandler = new Mock<IUnsupportedCommandMessageHandler>();
            var logger = new Mock<ILogger>();
            
            var handler = new KnownUserMessageHandler(cancelSubscriptionHandler.Object, unsupportedCommandHandler.Object, logger.Object);
            
            await handler.Handle("sample-subscriber-id", new Message
            {
                Mid = "sample-message-id",
                Text = "sample-message-text",
                QuickReply = new QuickReply{ Payload = new Payload(PayloadType.Cancel).ToJson() }
            });
            
            cancelSubscriptionHandler.Verify(x => x.Handle("sample-subscriber-id"), Times.Once);
            unsupportedCommandHandler.Verify(x => x.Handle("sample-subscriber-id"), Times.Never);
        }
        
        [Fact]
        public async void ShouldRouteRequestWithUnsupportedCommandToUnsupportedCommandHandler()
        {
            var cancelSubscriptionHandler = new Mock<ICancelSubscriptionMessageHandler>();
            var unsupportedCommandHandler = new Mock<IUnsupportedCommandMessageHandler>();
            var logger = new Mock<ILogger>();
            
            var handler = new KnownUserMessageHandler(cancelSubscriptionHandler.Object, unsupportedCommandHandler.Object, logger.Object);
            
            await handler.Handle("sample-subscriber-id", new Message
            {
                Mid = "sample-message-id",
                Text = "sample-message-text"
            });
            
            cancelSubscriptionHandler.Verify(x => x.Handle("sample-subscriber-id"), Times.Never);
            unsupportedCommandHandler.Verify(x => x.Handle("sample-subscriber-id"), Times.Once);
        }
    }
}