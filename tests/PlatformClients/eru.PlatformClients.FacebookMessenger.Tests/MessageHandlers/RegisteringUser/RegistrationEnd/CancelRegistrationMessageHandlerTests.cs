using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.CancelRegistration;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Message = eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Message;
using QuickReply = eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties.QuickReply;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUser.RegistrationEnd
{
    public class CancelRegistrationMessageHandlerTests
    {
        [Fact]
        public async void ShouldCancelRegistrationCorrectly()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();

            var message = new Messaging
            {
                Sender = new Sender {Id = "sample-registering-user"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text",
                    QuickReply = new QuickReply{Payload = new Payload(PayloadType.Cancel).ToJson()}
                }
            };
            
            var handler = new CancelRegistrationMessageHandler(context, apiClient.Object, MockBuilder.BuildFakeTranslator(), new Mock<ILogger<CancelRegistrationMessageHandler>>().Object);
            await handler.Handle(message);
            
            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user");
            
            var expectedMessage = new SendRequest("sample-registering-user", new FacebookMessenger.SendAPIClient.Requests.Message("subscription-cancelled-text"));
            apiClient.Verify(x => x.Send(
                It.Is<SendRequest>(y => y.IsEquivalentTo(expectedMessage))
                ));
            apiClient.VerifyNoOtherCalls();
        }
    }
}