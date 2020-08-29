using System.Threading;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Properties;
using MediatR;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.IncomingMessageHandler
{
    public class IncomingMessageHandlerTests
    {
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
        }

        [Fact]
        public async void ShouldHandleIncomingMessageFromRegisteringUserCorrectly()
        {
            var message = new Messaging
            {
                Sender = new Sender{Id = "registering-user-id"},
                Recipient = new Recipient{Id = "page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
            
            
        }

        [Fact]
        public async void ShouldHandleIncomingMessageFromSubscriberCorrectly()
        {
            var message = new Messaging
            {
                Sender = new Sender{Id = "subscriber-id"},
                Recipient = new Recipient{Id = "page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
        }
    }
}