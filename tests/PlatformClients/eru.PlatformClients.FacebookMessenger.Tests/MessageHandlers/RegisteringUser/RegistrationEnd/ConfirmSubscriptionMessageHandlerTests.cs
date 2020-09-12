﻿using System.Linq;
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
            var context = new FakeRegistrationDb();
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, BuildFakeTranslator(), new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
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

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, BuildFakeTranslator(), new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user-with-class"));

            client.Verify(x => x.Send(It.Is<SendRequest>(y => 
                y.Type == MessagingTypes.Response
                && y.Recipient.Id == "sample-registering-user-with-class"
                && y.Message.Text == "confirmation-text"
                && y.Message.QuickReplies.Count() == 2
                && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "subscribe-button-text" && z.Payload == new Payload(PayloadType.Subscribe).ToJson())
                && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                )));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();

            var handler = new ConfirmSubscriptionMessageHandler(context, mediator.Object, client.Object, BuildFakeTranslator(), new Mock<ILogger<ConfirmSubscriptionMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-class"), new Payload());
            
            client.Verify(x => x.Send(It.Is<SendRequest>(y => 
                y.Type == MessagingTypes.Response
                && y.Recipient.Id == "sample-registering-user-with-class"
                && y.Message.Text == "unsupported-command-text"
                && y.Message.QuickReplies.Count() == 2
                && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "subscribe-button-text" && z.Payload == new Payload(PayloadType.Subscribe).ToJson())
                && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
            )));
            client.VerifyNoOtherCalls();
        }

        private ITranslator<FacebookMessengerPlatformClient> BuildFakeTranslator()
        {
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            translator.Setup(x => x.TranslateString("congratulations", "en")).Returns(Task.FromResult("congratulations-text"));
            translator.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("cancel-button-text"));
            translator.Setup(x => x.TranslateString("subscribe-button", "en")).Returns(Task.FromResult("subscribe-button-text"));
            translator.Setup(x => x.TranslateString("unsupported-command", "en")).Returns(Task.FromResult("unsupported-command-text"));
            translator.Setup(x => x.TranslateString("confirmation", "en")).Returns(Task.FromResult("confirmation-text"));
            
            return translator.Object;
        }
    }
}