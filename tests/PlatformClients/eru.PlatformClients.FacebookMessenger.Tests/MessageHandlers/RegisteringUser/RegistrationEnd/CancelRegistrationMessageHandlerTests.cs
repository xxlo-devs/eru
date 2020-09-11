using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.CancelRegistration;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUser.RegistrationEnd
{
    public class CancelRegistrationMessageHandlerTests
    {
        [Fact]
        public async void ShouldCancelRegistrationCorrectly()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            var logger = new Mock<ILogger<CancelRegistrationMessageHandler>>();
            
            var handler = new CancelRegistrationMessageHandler(context, apiClient.Object, translator.Object, logger.Object);
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

            await handler.Handle(message);
            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user");
        }
    }
}