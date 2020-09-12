using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUser
{
    public class RegisteringUserMessageHandlerTests
    {
        [Fact]
        public async void ShouldRouteCancellationRequestCorrectly()
        {
            var builder = new RegisteringUserMessageHandlerBuilder();
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-registering-user"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 12345678,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text",
                    QuickReply = new QuickReply{Payload = new Payload(PayloadType.Cancel).ToJson()}
                }
            };

            await builder.RegisteringUserMessageHandler.Handle(message);
            
            builder.CancelRegistrationMessageHandlerMock.Verify(x => x.Handle(message), Times.Once);
            builder.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRouteRequestFromCreatedUserCorrectly()
        {
            var builder = new RegisteringUserMessageHandlerBuilder();
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-registering-user"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 12345678,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text",
                    QuickReply = new QuickReply{Payload = new Payload(PayloadType.Lang).ToJson()}
                }
            };

            await builder.RegisteringUserMessageHandler.Handle(message);

            var user = await builder.FakeRegistrationDb.IncompleteUsers.FindAsync("sample-registering-user");
            
            builder.GatherLanguageMessageHandlerMock.Verify(x => x.Handle(user, It.Is<Payload>(y => y.Type == PayloadType.Lang)));
            builder.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRouteRequestFromUserWithGatheredLanguageCorrectly()
        {
            var builder = new RegisteringUserMessageHandlerBuilder();
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-registering-user-with-lang"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 12345678,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text",
                    QuickReply = new QuickReply{Payload = new Payload(PayloadType.Year).ToJson()}
                }
            };

            await builder.RegisteringUserMessageHandler.Handle(message);

            var user = await builder.FakeRegistrationDb.IncompleteUsers.FindAsync("sample-registering-user-with-lang");
            
            builder.GatherYearMessageHandlerMock.Verify(x => x.Handle(user, It.Is<Payload>(y => y.Type == PayloadType.Year)));
            builder.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRouteRequestFromUserWithGatheredYearCorrectly()
        {
            var builder = new RegisteringUserMessageHandlerBuilder();
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-registering-user-with-year"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 12345678,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text",
                    QuickReply = new QuickReply{Payload = new Payload(PayloadType.Class).ToJson()}
                }
            };
            
            await builder.RegisteringUserMessageHandler.Handle(message);
            
            var user = await builder.FakeRegistrationDb.IncompleteUsers.FindAsync("sample-registering-user-with-year");
            
            builder.GatherClassMessageHandler.Verify(x => x.Handle(user, It.Is<Payload>(y => y.Type == PayloadType.Class)));
            builder.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRouteRequestFromUserWithGatheredClassCorrectly()
        {
            var builder = new RegisteringUserMessageHandlerBuilder();
            
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-registering-user-with-class"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 12345678,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text",
                    QuickReply = new QuickReply{Payload = new Payload(PayloadType.Subscribe).ToJson()}
                }
            };

            await builder.RegisteringUserMessageHandler.Handle(message);
            
            var user = await builder.FakeRegistrationDb.IncompleteUsers.FindAsync("sample-registering-user-with-class"); 
            
            builder.ConfirmSubscriptionMessageHandlerMock.Verify(x => x.Handle(user, It.Is<Payload>(y => y.Type == PayloadType.Subscribe)));
            builder.VerifyNoOtherCalls();
        }
    }
}