using System.Threading;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CreateSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
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
            var context = new FakeRegistrationDb();
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, translator.Object, new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-class"), new Payload(PayloadType.Subscribe));

            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user-with-class");
            mediator.Verify(x => x.Send(It.Is<CreateSubscriptionCommand>(y => y.Id == "sample-registering-user-with-class" && y.Platform == FacebookMessengerPlatformClient.PId && y.PreferredLanguage == "en" && y.Class == "sample-class"), It.IsAny<CancellationToken>()), Times.Once);
            mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowInstructionCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, translator.Object, new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user-with-class"));
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, translator.Object, new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
            await handler.UnsupportedCommand(await context.IncompleteUsers.FindAsync("sample-registering-user-with-class"));
        }
    }
}