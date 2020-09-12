using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Static;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUser.RegistrationSteps
{
    public class GatherClassMessageHandlerTests
    {
        [Fact]
        public async void ShouldUpdateUserCorrectly()
        {
            var context = new FakeRegistrationDb();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(MockBuilder.BuildMediatorMock().Object, new Mock<ISendApiClient>().Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherClassMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"), new Payload(PayloadType.Class, "sample-class-1a"));

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-year" && x.PreferredLanguage == "en" && x.Year == 1 &&
                x.ClassId == "sample-class-1a" && x.LastPage == 0 && x.Stage == Stage.GatheredClass);
            
            confirmHandler.Verify(x => x.ShowInstruction(It.Is<IncompleteUser>(y => y.Id == "sample-registering-user-with-year")), Times.Once);
            confirmHandler.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowListPageCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherClassMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"), new Payload(PayloadType.Class, 1));
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user-with-year"
                     && y.Message.Text == "class-selection-text"
                     && y.Message.QuickReplies.Count() == 4
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "<-" && z.Payload == new Payload(PayloadType.Class, 0).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1k" && z.Payload == new Payload(PayloadType.Class, "sample-class-1k").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1l" && z.Payload == new Payload(PayloadType.Class, "sample-class-1l").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowInstructionCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherClassMessageHandler>());
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"));
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user-with-year"
                     && y.Message.Text == "class-selection-text"
                     && y.Message.QuickReplies.Count() == 12
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "->" && z.Payload == new Payload(PayloadType.Class, 1).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1a" && z.Payload == new Payload(PayloadType.Class, "sample-class-1a").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1b" && z.Payload == new Payload(PayloadType.Class, "sample-class-1b").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1c" && z.Payload == new Payload(PayloadType.Class, "sample-class-1c").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1d" && z.Payload == new Payload(PayloadType.Class, "sample-class-1d").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1e" && z.Payload == new Payload(PayloadType.Class, "sample-class-1e").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1f" && z.Payload == new Payload(PayloadType.Class, "sample-class-1f").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1g" && z.Payload == new Payload(PayloadType.Class, "sample-class-1g").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1h" && z.Payload == new Payload(PayloadType.Class, "sample-class-1h").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1i" && z.Payload == new Payload(PayloadType.Class, "sample-class-1i").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1j" && z.Payload == new Payload(PayloadType.Class, "sample-class-1j").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherClassMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"), new Payload());
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user-with-year"
                     && y.Message.Text == "unsupported-command-text"
                     && y.Message.QuickReplies.Count() == 12
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "->" && z.Payload == new Payload(PayloadType.Class, 1).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1a" && z.Payload == new Payload(PayloadType.Class, "sample-class-1a").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1b" && z.Payload == new Payload(PayloadType.Class, "sample-class-1b").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1c" && z.Payload == new Payload(PayloadType.Class, "sample-class-1c").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1d" && z.Payload == new Payload(PayloadType.Class, "sample-class-1d").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1e" && z.Payload == new Payload(PayloadType.Class, "sample-class-1e").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1f" && z.Payload == new Payload(PayloadType.Class, "sample-class-1f").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1g" && z.Payload == new Payload(PayloadType.Class, "sample-class-1g").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1h" && z.Payload == new Payload(PayloadType.Class, "sample-class-1h").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1i" && z.Payload == new Payload(PayloadType.Class, "sample-class-1i").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1j" && z.Payload == new Payload(PayloadType.Class, "sample-class-1j").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }
        

    }
}