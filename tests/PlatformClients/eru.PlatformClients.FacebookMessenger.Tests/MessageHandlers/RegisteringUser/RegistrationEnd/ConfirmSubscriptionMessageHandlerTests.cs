using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CreateSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Static;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
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
            translator.Setup(x => x.TranslateString("congratulations", "en")).Returns(Task.FromResult("congratulations-text"));
            translator.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("cancel-button-text"));

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, translator.Object, new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-class"), new Payload(PayloadType.Subscribe));

            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user-with-class");
            mediator.Verify(x => x.Send(It.Is<CreateSubscriptionCommand>(y => y.Id == "sample-registering-user-with-class" && y.Platform == FacebookMessengerPlatformClient.PId && y.PreferredLanguage == "en" && y.Class == "sample-class"), It.IsAny<CancellationToken>()), Times.Once);
            mediator.VerifyNoOtherCalls();

            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user-with-class"
                     && y.Message.Text == "congratulations-text"
                     && y.Message.QuickReplies.Count() == 1
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == "{\"Type\":\"Cancel\"}")
                     )));
            client.VerifyNoOtherCalls();
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

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-class" && x.PreferredLanguage == "en" && x.Year == 1 &&
                x.ClassId == "sample-class" && x.LastPage == 0 && x.Stage == Stage.GatheredClass);
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, translator.Object, new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-class"), new Payload());
        }
    }
}