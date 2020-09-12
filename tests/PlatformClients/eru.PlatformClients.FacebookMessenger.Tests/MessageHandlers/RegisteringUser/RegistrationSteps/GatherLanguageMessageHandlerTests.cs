using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Static;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUser.RegistrationSteps
{
    public class GatherLanguageMessageHandlerTests
    {
        [Fact]
        public async void ShouldUpdateUserCorrectly()
        {
            var context = new FakeRegistrationDb();
            var client = new Mock<ISendApiClient>();
            var yearHandler = new Mock<IGatherYearMessageHandler>();

            var handler = new GatherLanguageMessageHandler(MockBuilder.BuildFakeConfiguration(), client.Object, MockBuilder.BuildFakeTranslator(), yearHandler.Object, context, MockBuilder.BuildFakeLogger<GatherLanguageMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user"), new Payload(PayloadType.Lang, "pl"));

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user" && x.PreferredLanguage == "pl" && x.LastPage == 0 &&
                x.Stage == Stage.GatheredLanguage);
            
            yearHandler.Verify(x => x.ShowInstruction(It.Is<IncompleteUser>(y => y.Id == "sample-registering-user"), 0), Times.Once);
            yearHandler.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowListPageCorrectly()
        {
            var context = new FakeRegistrationDb();
            var client = new Mock<ISendApiClient>();
            var yearHandler = new Mock<IGatherYearMessageHandler>();

            var handler = new GatherLanguageMessageHandler(MockBuilder.BuildFakeConfiguration(), client.Object, MockBuilder.BuildFakeTranslator(), yearHandler.Object, context, MockBuilder.BuildFakeLogger<GatherLanguageMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user"), new Payload(PayloadType.Lang, 0));
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user"
                     && y.Message.Text == "greeting-text"
                     && y.Message.QuickReplies.Count() == 3
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == new CultureInfo("en").DisplayName && z.Payload == new Payload(PayloadType.Lang, "en").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == new CultureInfo("pl").DisplayName && z.Payload == new Payload(PayloadType.Lang, "pl").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowInstructionCorrectly()
        {
            var context = new FakeRegistrationDb();
            var client = new Mock<ISendApiClient>();
            var yearHandler = new Mock<IGatherYearMessageHandler>();

            var handler = new GatherLanguageMessageHandler(MockBuilder.BuildFakeConfiguration(), client.Object, MockBuilder.BuildFakeTranslator(), yearHandler.Object, context, MockBuilder.BuildFakeLogger<GatherLanguageMessageHandler>());
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user"));

            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user"
                     && y.Message.Text == "greeting-text"
                     && y.Message.QuickReplies.Count() == 3
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == new CultureInfo("en").DisplayName && z.Payload == new Payload(PayloadType.Lang, "en").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == new CultureInfo("pl").DisplayName && z.Payload == new Payload(PayloadType.Lang, "pl").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var context = new FakeRegistrationDb();
            var client = new Mock<ISendApiClient>();
            var yearHandler = new Mock<IGatherYearMessageHandler>();

            var handler = new GatherLanguageMessageHandler(MockBuilder.BuildFakeConfiguration(), client.Object, MockBuilder.BuildFakeTranslator(), yearHandler.Object, context, MockBuilder.BuildFakeLogger<GatherLanguageMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user"), new Payload());
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-registering-user"
                     && y.Message.Text == "unsupported-command-text"
                     && y.Message.QuickReplies.Count() == 3
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == new CultureInfo("en").DisplayName && z.Payload == new Payload(PayloadType.Lang, "en").ToJson())
                     && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == new CultureInfo("pl").DisplayName && z.Payload == new Payload(PayloadType.Lang, "pl").ToJson())
            )));
            client.VerifyNoOtherCalls();
        }
    }
}