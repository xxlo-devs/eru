using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear;
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
    public class GatherYearMessageHandlerTests
    {
        [Fact]
        public async void ShouldUpdateUserCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IGatherClassMessageHandler>();

            var handler = new GatherYearMessageHandler(mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherYearMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-lang"), new Payload(PayloadType.Year, "1"));

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.PreferredLanguage == "en" && x.Year == 1 &&
                x.LastPage == 0 && x.Stage == Stage.GatheredYear);
            
            confirmHandler.Verify(x => x.ShowInstruction(It.Is<IncompleteUser>(y => y.Id == "sample-registering-user-with-lang"), 0), Times.Once);
            confirmHandler.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowListPageCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IGatherClassMessageHandler>();

            var handler = new GatherYearMessageHandler(mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherYearMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-lang"), new Payload(PayloadType.Year, 1));
                        
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user-with-lang"
                     && y.Message.Text == "year-selection-text"
                     && y.Message.QuickReplies.Count() == 4
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "<-" && z.Payload == new Payload(PayloadType.Year, 0).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "11" && z.Payload == new Payload(PayloadType.Year, "11").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "12" && z.Payload == new Payload(PayloadType.Year, "12").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowInstructionCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IGatherClassMessageHandler>();

            var handler = new GatherYearMessageHandler(mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherYearMessageHandler>());
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user-with-lang"));
            
            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.PreferredLanguage == "en" &&
                x.LastPage == 0 && x.Stage == Stage.GatheredLanguage);
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user-with-lang"
                     && y.Message.Text == "year-selection-text"
                     && y.Message.QuickReplies.Count() == 12
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "->" && z.Payload == new Payload(PayloadType.Year, 1).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1" && z.Payload == new Payload(PayloadType.Year, "1").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "2" && z.Payload == new Payload(PayloadType.Year, "2").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "3" && z.Payload == new Payload(PayloadType.Year, "3").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "4" && z.Payload == new Payload(PayloadType.Year, "4").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "5" && z.Payload == new Payload(PayloadType.Year, "5").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "6" && z.Payload == new Payload(PayloadType.Year, "6").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "7" && z.Payload == new Payload(PayloadType.Year, "7").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "8" && z.Payload == new Payload(PayloadType.Year, "8").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "9" && z.Payload == new Payload(PayloadType.Year, "9").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "10" && z.Payload == new Payload(PayloadType.Year, "10").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IGatherClassMessageHandler>();

            var handler = new GatherYearMessageHandler(mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherYearMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-lang"), new Payload());
            
            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.PreferredLanguage == "en" &&
                x.LastPage == 0 && x.Stage == Stage.GatheredLanguage);
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user-with-lang"
                     && y.Message.Text == "unsupported-command-text"
                     && y.Message.QuickReplies.Count() == 12
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "->" && z.Payload == new Payload(PayloadType.Year, 1).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "1" && z.Payload == new Payload(PayloadType.Year, "1").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "2" && z.Payload == new Payload(PayloadType.Year, "2").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "3" && z.Payload == new Payload(PayloadType.Year, "3").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "4" && z.Payload == new Payload(PayloadType.Year, "4").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "5" && z.Payload == new Payload(PayloadType.Year, "5").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "6" && z.Payload == new Payload(PayloadType.Year, "6").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "7" && z.Payload == new Payload(PayloadType.Year, "7").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "8" && z.Payload == new Payload(PayloadType.Year, "8").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "9" && z.Payload == new Payload(PayloadType.Year, "9").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "10" && z.Payload == new Payload(PayloadType.Year, "10").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }
        
        
    }
}