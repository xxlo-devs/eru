using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CreateSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Static;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUser.RegistrationEnd
{
    public class ConfirmSubscriptionMessageHandlerTests
    {
        [Fact]
        public async void ShouldCreateSubscriptionCorrectly()
        {
            var mediator = MockBuilder.BuildMediatorMock();
            var context = new FakeRegistrationDb();
            var client = new Mock<ISendApiClient>();

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-class"), new Payload(PayloadType.Subscribe));

            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user-with-class");
            
            mediator.Verify(x => x.Send(It.Is<CreateSubscriptionCommand>(y => y.Id == "sample-registering-user-with-class" && y.Platform == FacebookMessengerPlatformClient.PId && y.PreferredLanguage == "en" && y.Class == "sample-class"), It.IsAny<CancellationToken>()), Times.Once);
            mediator.VerifyNoOtherCalls();

            var expectedMessage = new SendRequest("sample-registering-user-with-class", new Message("congratulations-text", new[]
            {
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson())
            }));
            client.Verify(x => x.Send(It.Is<SendRequest>(
                    y => y.IsEquivalentTo(expectedMessage))
                )
            );
        }

        [Fact]
        public async void ShouldShowInstructionCorrectly()
        {
            var mediator = MockBuilder.BuildMediatorMock();
            var context = new FakeRegistrationDb();

            var client = new Mock<ISendApiClient>();

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user-with-class"));

            var expectedMessage = new SendRequest("sample-registering-user-with-class", new Message("confirmation-text", new[]
            {
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson()),
                new QuickReply("subscribe-button-text", new Payload(PayloadType.Subscribe).ToJson()) 
            }));
            client.Verify(x => x.Send(It.Is<SendRequest>(
                    y => y.IsEquivalentTo(expectedMessage))
                )
            );
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var mediator = MockBuilder.BuildMediatorMock();
            var context = new FakeRegistrationDb();

            var client = new Mock<ISendApiClient>();

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-class"), new Payload());
            
            var expectedMessage = new SendRequest("sample-registering-user-with-class", new Message("unsupported-command-text", new[]
            {
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson()),
                new QuickReply("subscribe-button-text", new Payload(PayloadType.Subscribe).ToJson()) 
            }));
            client.Verify(x => x.Send(It.Is<SendRequest>(
                    y => y.IsEquivalentTo(expectedMessage))
                )
            );
            client.VerifyNoOtherCalls();
        }
    }
}