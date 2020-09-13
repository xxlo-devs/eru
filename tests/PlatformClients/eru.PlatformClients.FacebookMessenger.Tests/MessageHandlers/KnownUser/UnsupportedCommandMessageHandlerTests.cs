using System.Threading;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using Moq;
using Xunit;
using Message = eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Message;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.KnownUser
{
    public class UnsupportedCommandMessageHandlerTests
    {
        [Fact]
        public async void ShouldHandleRequestWithUnsupportedCommandCorrectly()
        {
            var mediator = MockBuilder.BuildMediatorMock();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new UnsupportedCommandMessageHandler(mediator.Object, apiClient.Object,
                MockBuilder.BuildFakeTranslator(), MockBuilder.BuildFakeLogger<UnsupportedCommandMessageHandler>());
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
            
            mediator.Verify(
                x => x.Send(
                    It.Is<GetSubscriberQuery>(y =>
                        y.Id == "sample-subscriber" && y.Platform == FacebookMessengerPlatformClient.PId),
                    It.IsAny<CancellationToken>()), Times.Once);
            mediator.VerifyNoOtherCalls();
            
            var expectedMessage = new SendRequest(
                "sample-subscriber",
                new FacebookMessenger.SendAPIClient.Requests.Message("unsupported-command-text", new[]
                {
                    new FacebookMessenger.SendAPIClient.Requests.QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson())
                })
                );
            apiClient.Verify(x => x.Send(
                It.Is<SendRequest>(y => y.IsEquivalentTo(expectedMessage))
                ));
        }
    }
}