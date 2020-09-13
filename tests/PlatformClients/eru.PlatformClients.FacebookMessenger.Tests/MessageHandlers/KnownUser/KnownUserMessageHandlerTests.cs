using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.KnownUser
{
    public class KnownUserMessageHandlerTests
    {
        [Fact]
        public async void ShouldRouteCancelSubscriptionRequestCorrectly()
        {
            var cancelHandler = new Mock<ICancelSubscriptionMessageHandler>();
            var unsupportedHandler = new Mock<IUnsupportedCommandMessageHandler>();
            
            var message = new Messaging
            {
                Sender = new Sender {Id = "sample-subscriber"},
                Recipient = new Recipient {Id = "sample-page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text",
                    QuickReply = new QuickReply{Payload = new Payload(PayloadType.Cancel).ToJson()}
                }
            };
            
            var handler = new KnownUserMessageMessageHandler(cancelHandler.Object, unsupportedHandler.Object,
                MockBuilder.BuildFakeLogger<KnownUserMessageMessageHandler>());
            await handler.Handle(message);
            
            cancelHandler.Verify(x => x.Handle(message));
            cancelHandler.VerifyNoOtherCalls();
            unsupportedHandler.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRouteRequestWithUnsupportedCommandCorrectly()
        {
            var cancelHandler = new Mock<ICancelSubscriptionMessageHandler>();
            var unsupportedHandler = new Mock<IUnsupportedCommandMessageHandler>();

            var message = new Messaging
            {
                Sender = new Sender {Id = "sample-subscriber"},
                Recipient = new Recipient {Id = "sample-page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
            
            var handler = new KnownUserMessageMessageHandler(cancelHandler.Object, unsupportedHandler.Object,
                MockBuilder.BuildFakeLogger<KnownUserMessageMessageHandler>());
            await handler.Handle(message);
            
            unsupportedHandler.Verify(x => x.Handle(message));
            unsupportedHandler.VerifyNoOtherCalls();
            cancelHandler.VerifyNoOtherCalls();
        }
    }
}