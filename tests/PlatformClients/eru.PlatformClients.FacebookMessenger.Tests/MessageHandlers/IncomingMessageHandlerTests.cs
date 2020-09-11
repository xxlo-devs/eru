using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers
{
    public class IncomingMessageHandlerTests
    {
        [Fact]
        public async void ShouldRouteMessageFromKnownUserCorrectly()
        {
            var builder = new IncomingMessageHandlerBuilder();

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
            
            await builder.IncomingMessageHandler.Handle(message);
            
            builder.KnownUserMessageHandlerMock.Verify(x => x.Handle(message), Times.Once);
            builder.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRouteMessageFromRegisteringUserCorrectly()
        {
            var builder = new IncomingMessageHandlerBuilder();

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
            
            await builder.IncomingMessageHandler.Handle(message);
            
            builder.RegisteringUserMessageHandlerMock.Verify(x => x.Handle(message), Times.Once);
            builder.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldRouteMessageFromUnknownUserCorrectly()
        {
            var builder = new IncomingMessageHandlerBuilder();

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
            
            await builder.IncomingMessageHandler.Handle(message);
            
            builder.UnknownUserMessageHandlerMock.Verify(x => x.Handle(message), Times.Once);
            builder.VerifyNoOtherCalls();
        }
    }
}