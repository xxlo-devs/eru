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
using Microsoft.EntityFrameworkCore.Internal;
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

            var handler = new GatherClassMessageHandler(new Mock<IMediator>().Object, new Mock<ISendApiClient>().Object, new Mock<ITranslator<FacebookMessengerPlatformClient>>().Object, confirmHandler.Object, context, new Mock<ILogger<GatherClassMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"), new Payload(PayloadType.Class, "sample-class"));

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-year" && x.PreferredLanguage == "en" && x.Year == 1 &&
                x.ClassId == "sample-class" && x.LastPage == 0 && x.Stage == Stage.GatheredClass);
            
            confirmHandler.Verify(x => x.ShowInstruction(It.Is<IncompleteUser>(y => y.Id == "sample-registering-user-with-year")), Times.Once);
            confirmHandler.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowListPageCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = BuildFakeMediator();
            var client = new Mock<ISendApiClient>();
            var translator = BuildFakeTranslator();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, translator.Object, confirmHandler.Object, context, new Mock<ILogger<GatherClassMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"), new Payload(PayloadType.Class, 1));
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user-with-year"
                     && y.Message.Text == "class-selection-text"
                     && y.Message.QuickReplies.Count() == 4
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "<-" && z.Payload == new Payload(PayloadType.Class, 0).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1k" && z.Payload == new Payload(PayloadType.Class, "sample-class-11").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1l" && z.Payload == new Payload(PayloadType.Class, "sample-class-12").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowInstructionCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = BuildFakeMediator();
            var client = new Mock<ISendApiClient>();
            var translator = BuildFakeTranslator();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, translator.Object, confirmHandler.Object, context, new Mock<ILogger<GatherClassMessageHandler>>().Object);
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"));
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user-with-year"
                     && y.Message.Text == "class-selection-text"
                     && y.Message.QuickReplies.Count() == 12
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "->" && z.Payload == new Payload(PayloadType.Class, 1).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1a" && z.Payload == new Payload(PayloadType.Class, "sample-class-1").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1b" && z.Payload == new Payload(PayloadType.Class, "sample-class-2").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1c" && z.Payload == new Payload(PayloadType.Class, "sample-class-3").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1d" && z.Payload == new Payload(PayloadType.Class, "sample-class-4").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1e" && z.Payload == new Payload(PayloadType.Class, "sample-class-5").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1f" && z.Payload == new Payload(PayloadType.Class, "sample-class-6").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1g" && z.Payload == new Payload(PayloadType.Class, "sample-class-7").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1h" && z.Payload == new Payload(PayloadType.Class, "sample-class-8").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1i" && z.Payload == new Payload(PayloadType.Class, "sample-class-9").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1j" && z.Payload == new Payload(PayloadType.Class, "sample-class-10").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = BuildFakeMediator();
            var client = new Mock<ISendApiClient>();
            var translator = BuildFakeTranslator();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, translator.Object, confirmHandler.Object, context, new Mock<ILogger<GatherClassMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"), new Payload());
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user-with-year"
                     && y.Message.Text == "unsupported-command-text"
                     && y.Message.QuickReplies.Count() == 12
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "->" && z.Payload == new Payload(PayloadType.Class, 1).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1a" && z.Payload == new Payload(PayloadType.Class, "sample-class-1").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1b" && z.Payload == new Payload(PayloadType.Class, "sample-class-2").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1c" && z.Payload == new Payload(PayloadType.Class, "sample-class-3").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1d" && z.Payload == new Payload(PayloadType.Class, "sample-class-4").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1e" && z.Payload == new Payload(PayloadType.Class, "sample-class-5").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1f" && z.Payload == new Payload(PayloadType.Class, "sample-class-6").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1g" && z.Payload == new Payload(PayloadType.Class, "sample-class-7").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1h" && z.Payload == new Payload(PayloadType.Class, "sample-class-8").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1i" && z.Payload == new Payload(PayloadType.Class, "sample-class-9").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1j" && z.Payload == new Payload(PayloadType.Class, "sample-class-10").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }
        
        private Mock<IMediator> BuildFakeMediator()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GetClassesQuery>(), It.IsAny<CancellationToken>())).Returns(
                Task.FromResult(new[]
                {
                    new ClassDto {Id = "sample-class-1", Year = 1, Section = "a"},
                    new ClassDto {Id = "sample-class-2", Year = 1, Section = "b"},
                    new ClassDto {Id = "sample-class-3", Year = 1, Section = "c"},
                    new ClassDto {Id = "sample-class-4", Year = 1, Section = "d"},
                    new ClassDto {Id = "sample-class-5", Year = 1, Section = "e"},
                    new ClassDto {Id = "sample-class-6", Year = 1, Section = "f"},
                    new ClassDto {Id = "sample-class-7", Year = 1, Section = "g"},
                    new ClassDto {Id = "sample-class-8", Year = 1, Section = "h"},
                    new ClassDto {Id = "sample-class-9", Year = 1, Section = "i"},
                    new ClassDto {Id = "sample-class-10", Year = 1, Section = "j"},
                    new ClassDto {Id = "sample-class-11", Year = 1, Section = "k"},
                    new ClassDto {Id = "sample-class-12", Year = 1, Section = "l"}
                }.AsEnumerable()));
            return mediator;
        }

        private Mock<ITranslator<FacebookMessengerPlatformClient>> BuildFakeTranslator()
        {
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            
            translator.Setup(x => x.TranslateString("next-page", "en")).Returns(Task.FromResult("->"));
            translator.Setup(x => x.TranslateString("previous-page", "en")).Returns(Task.FromResult("<-"));
            translator.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("cancel-button-text"));
            translator.Setup(x => x.TranslateString("unsupported-command", "en")).Returns(Task.FromResult("unsupported-command-text"));
            translator.Setup(x => x.TranslateString("class-selection", "en")).Returns(Task.FromResult("class-selection-text"));
            
            return translator;
        }
    }
}