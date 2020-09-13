using System.Globalization;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using FluentAssertions;
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

            var handler = new GatherLanguageMessageHandler(MockBuilder.BuildFakeConfiguration(), client.Object,
                MockBuilder.BuildFakeTranslator(), yearHandler.Object, context,
                MockBuilder.BuildFakeLogger<GatherLanguageMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user"), new Payload(PayloadType.Lang, "pl"));

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user" && x.PreferredLanguage == "pl" && x.LastPage == 0 &&
                x.Stage == Stage.GatheredLanguage);
            
            yearHandler.Verify(x 
                => x.ShowInstruction(It.Is<IncompleteUser>(y => y.Id == "sample-registering-user"), 0), Times.Once);
            yearHandler.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowListPageCorrectly()
        {
            var context = new FakeRegistrationDb();
            var client = new Mock<ISendApiClient>();
            var yearHandler = new Mock<IGatherYearMessageHandler>();

            var handler = new GatherLanguageMessageHandler(MockBuilder.BuildFakeConfiguration(), client.Object,
                MockBuilder.BuildFakeTranslator(), yearHandler.Object, context,
                MockBuilder.BuildFakeLogger<GatherLanguageMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user"), new Payload(PayloadType.Lang, 0));
            
            var expectedMessage = new SendRequest("sample-registering-user", new Message("greeting-text", new[] 
            {
                new QuickReply("en", new Payload(PayloadType.Lang, "en").ToJson()), 
                new QuickReply("pl", new Payload(PayloadType.Lang, "pl").ToJson()), 
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson())
            }));
            
            client.Verify(x => x.Send(
                It.Is<SendRequest>(y => y.IsEquivalentTo(expectedMessage))
                ));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowInstructionCorrectly()
        {
            var context = new FakeRegistrationDb();
            var client = new Mock<ISendApiClient>();
            var yearHandler = new Mock<IGatherYearMessageHandler>();

            var handler = new GatherLanguageMessageHandler(MockBuilder.BuildFakeConfiguration(), client.Object,
                MockBuilder.BuildFakeTranslator(), yearHandler.Object, context,
                MockBuilder.BuildFakeLogger<GatherLanguageMessageHandler>());
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user"));

            var expectedMessage = new SendRequest("sample-registering-user", new Message("greeting-text", new[] 
            {
                new QuickReply("en", new Payload(PayloadType.Lang, "en").ToJson()), 
                new QuickReply("pl", new Payload(PayloadType.Lang, "pl").ToJson()), 
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson())
            }));
            
            client.Verify(x => x.Send(
                It.Is<SendRequest>(y => y.IsEquivalentTo(expectedMessage))
                ));
            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var context = new FakeRegistrationDb();
            var client = new Mock<ISendApiClient>();
            var yearHandler = new Mock<IGatherYearMessageHandler>();

            var handler = new GatherLanguageMessageHandler(MockBuilder.BuildFakeConfiguration(), client.Object,
                MockBuilder.BuildFakeTranslator(), yearHandler.Object, context,
                MockBuilder.BuildFakeLogger<GatherLanguageMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user"), new Payload());
            
            var expectedMessage = new SendRequest("sample-registering-user", new Message(
                "unsupported-command-text", new[]
                {
                    new QuickReply("en", new Payload(PayloadType.Lang, "en").ToJson()), 
                    new QuickReply("pl", new Payload(PayloadType.Lang, "pl").ToJson()),  
                    new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson())
                }));
            
            client.Verify(x => x.Send(
                It.Is<SendRequest>(y => y.IsEquivalentTo(expectedMessage))
                ));
            client.VerifyNoOtherCalls();
        }
    }
}