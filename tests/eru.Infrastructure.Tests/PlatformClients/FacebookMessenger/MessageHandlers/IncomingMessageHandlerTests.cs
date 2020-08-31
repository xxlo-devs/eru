using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Properties;
using MediatR;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers
{
    public class IncomingMessageHandlerTests
    {
        private IMediator BuildFakeMediator()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>()))
                .Returns((GetSubscriberQuery query, CancellationToken cancellationToken) =>
                {
                    if (query.Id == "sample-subscriber-id" && query.Platform == "FacebookMessenger")
                    {
                        return Task.FromResult(new Subscriber
                        {
                            Id = "sample-subscriber-id",
                            Platform = "FacebookMessenger",
                            Class = "sample-class",
                            PreferredLanguage = "en-us"
                        });
                    }
                    else
                    {
                        return Task.FromResult<Subscriber>(null);
                    }
                });

            return mediator.Object;
        }
        
        [Fact]
        public async void ShouldHandleIncomingMessageFromUnknownUserCorrectly()
        {
            var message = new Messaging
            {
                Sender = new Sender{Id = "unknown-user-id"},
                Recipient = new Recipient{Id = "page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
            
            var dbContext = new FakeRegistrationDb();
            var mediator = BuildFakeMediator();

            var knownUserMessageHandler = new Mock<IKnownUserMessageHandler>(); 
            var registeringUserMessageHandler = new Mock<IRegisteringUserMessageHandler>(); 
            var unknownUserMessageHandler = new Mock<IUnkownUserMessageHandler>(); 
            
            var messageHandler = new IncomingMessageHandler(knownUserMessageHandler.Object, registeringUserMessageHandler.Object, unknownUserMessageHandler.Object, mediator, dbContext);

            await messageHandler.Handle(message);
            
            knownUserMessageHandler.Verify(x => x.Handle("sample-subscriber-id", It.IsAny<Message>()), Times.Never);
            registeringUserMessageHandler.Verify(x => x.Handle("sample-registering-user", It.IsAny<Message>()), Times.Never);
            unknownUserMessageHandler.Verify(x => x.Handle("unknown-user-id"), Times.Exactly(1));
        }

        [Fact]
        public async void ShouldHandleIncomingMessageFromRegisteringUserCorrectly()
        {
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-registering-user"},
                Recipient = new Recipient{Id = "page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
            
            var dbContext = new FakeRegistrationDb();
            var mediator = BuildFakeMediator();
            
            var knownUserMessageHandler = new Mock<IKnownUserMessageHandler>();
            var registeringUserMessageHandler = new Mock<IRegisteringUserMessageHandler>();
            var unknownUserMessageHandler = new Mock<IUnkownUserMessageHandler>();
            
            var messageHandler = new IncomingMessageHandler(knownUserMessageHandler.Object, registeringUserMessageHandler.Object, unknownUserMessageHandler.Object, mediator, dbContext);

            await messageHandler.Handle(message);
            
            knownUserMessageHandler.Verify(x => x.Handle("sample-subscriber-id", It.IsAny<Message>()), Times.Never);
            registeringUserMessageHandler.Verify(x => x.Handle("sample-registering-user", It.IsAny<Message>()), Times.Exactly(1));
            unknownUserMessageHandler.Verify(x => x.Handle("unknown-user-id"), Times.Never);
        }

        [Fact]
        public async void ShouldHandleIncomingMessageFromSubscriberCorrectly()
        {
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-subscriber-id"},
                Recipient = new Recipient{Id = "page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
            
            var dbContext = new FakeRegistrationDb();
            var mediator = BuildFakeMediator();
            var knownUserMessageHandler = new Mock<IKnownUserMessageHandler>();
            var registeringUserMessageHandler = new Mock<IRegisteringUserMessageHandler>();
            var unknownUserMessageHandler = new Mock<IUnkownUserMessageHandler>();
            
            var messageHandler = new IncomingMessageHandler(knownUserMessageHandler.Object, registeringUserMessageHandler.Object, unknownUserMessageHandler.Object, mediator, dbContext);

            await messageHandler.Handle(message);
            
            knownUserMessageHandler.Verify(x => x.Handle("sample-subscriber-id", It.IsAny<Message>()), Times.Exactly(1));
            registeringUserMessageHandler.Verify(x => x.Handle("sample-registering-user", It.IsAny<Message>()), Times.Never);
            unknownUserMessageHandler.Verify(x => x.Handle("unknown-user-id"), Times.Never);
        }
    }
}