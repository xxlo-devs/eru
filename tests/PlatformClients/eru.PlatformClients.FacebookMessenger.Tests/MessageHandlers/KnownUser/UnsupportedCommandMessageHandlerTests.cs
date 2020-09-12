using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Static;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Message = eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Message;
using QuickReply = eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.QuickReply;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.KnownUser
{
    public class UnsupportedCommandMessageHandlerTests
    {
        [Fact]
        public async void ShouldHandleRequestWithUnsupportedCommandCorrectly()
        {
            var mediator = BuildMediatorMock();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new UnsupportedCommandMessageHandler(mediator.Object, apiClient.Object, BuildFakeTranslator(), new Mock<ILogger<UnsupportedCommandMessageHandler>>().Object);
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

            await handler.Handle(message);
            
            mediator.Verify(x => x.Send(It.Is<GetSubscriberQuery>(y => y.Id == "sample-subscriber" && y.Platform == FacebookMessengerPlatformClient.PId), It.IsAny<CancellationToken>()), Times.Once);
            mediator.VerifyNoOtherCalls();
        }

        private Mock<IMediator> BuildMediatorMock()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>())).Returns(
                (GetSubscriberQuery query, CancellationToken cancellationToken) =>
                {
                    if (query.Id == "sample-subscriber" && query.Platform == FacebookMessengerPlatformClient.PId)
                        return Task.FromResult(new Subscriber
                        {
                            Id = "sample-subscriber", Platform = FacebookMessengerPlatformClient.PId,
                            PreferredLanguage = "en", Class = "sample-class"
                        });
                    else
                    {
                        return Task.FromResult<Subscriber>(null);
                    }
                });
            return mediator;
        }
        private ITranslator<FacebookMessengerPlatformClient> BuildFakeTranslator()
        {
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            translator.Setup(x => x.TranslateString("unsupported-command", "en")).Returns(Task.FromResult("unsupported-command-text"));
            translator.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("cancel-button-text"));
            return translator.Object;
        }
    }
}