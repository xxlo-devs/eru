using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Static;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Message = eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Message;
using QuickReply = eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties.QuickReply;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.KnownUser
{
    public class CancelSubscriptionMessageHandlerTests
    {
        [Fact]
        public async void ShouldCancelSubscriptionCorrectly()
        {
            var mediator = MockBuilder.BuildMediatorMock();
            var apiClient = new Mock<ISendApiClient>();

            var message = new Messaging
            {
                Sender = new Sender {Id = "sample-subscriber"},
                Recipient = new Recipient {Id = "sample-page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "Cancel",
                    QuickReply = new QuickReply {Payload = new Payload(PayloadType.Cancel).ToJson()}
                }
            };
            
            var handler = new CancelSubscriptionMessageHandler(mediator.Object, apiClient.Object, MockBuilder.BuildFakeTranslator(), MockBuilder.BuildFakeLogger<CancelSubscriptionMessageHandler>());
            await handler.Handle(message);

            mediator.Verify(x => x.Send(It.Is<GetSubscriberQuery>(y => y.Id == "sample-subscriber" && y.Platform == FacebookMessengerPlatformClient.PId), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(x => x.Send(It.Is<CancelSubscriptionCommand>(y => y.Id == "sample-subscriber" && y.Platform == FacebookMessengerPlatformClient.PId), It.IsAny<CancellationToken>()), Times.Once);
            mediator.VerifyNoOtherCalls();
            
            apiClient.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-subscriber"
                     && y.Message.Text == "subscription-cancelled-text"
                     && y.Message.QuickReplies == null
            )));
            apiClient.VerifyNoOtherCalls();
        }
    }
}