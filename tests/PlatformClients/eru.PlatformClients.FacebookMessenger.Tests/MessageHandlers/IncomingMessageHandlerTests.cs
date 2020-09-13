using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers
{
    public class IncomingMessageHandlerTests
    {
        [Fact]
        public async void ShouldRouteMessageFromKnownUserCorrectly()
        {
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-subscriber"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
            
            var builder = new IncomingMessageHandlerBuilder();
            await builder.IncomingMessageHandler.Handle(message);
            
            builder.KnownUserMessageHandlerMock.Verify(x => x.Handle(message), Times.Once);
            builder.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRouteMessageFromRegisteringUserCorrectly()
        {
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-registering-user"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
            
            var builder = new IncomingMessageHandlerBuilder();
            await builder.IncomingMessageHandler.Handle(message);
            
            builder.RegisteringUserMessageHandlerMock.Verify(x => x.Handle(message), Times.Once);
            builder.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRouteMessageFromUnknownUserCorrectly()
        {
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-unknown-user"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
         
            var builder = new IncomingMessageHandlerBuilder();
            await builder.IncomingMessageHandler.Handle(message);
            
            builder.UnknownUserMessageHandlerMock.Verify(x => x.Handle(message), Times.Once);
            builder.VerifyNoOtherCalls();
        }
    }
}