using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
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
    public class GatherYearMessageHandlerTests
    {
        [Fact]
        public async void ShouldUpdateUserCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IGatherClassMessageHandler>();

            var handler = new GatherYearMessageHandler(mediator.Object, client.Object,
                MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context,
                MockBuilder.BuildFakeLogger<GatherYearMessageHandler>(), MockBuilder.BuildFakeCultures());
            await handler.Handle(
                await context.IncompleteUsers.FindAsync("sample-registering-user-with-lang"), 
                new Payload(PayloadType.Year, "1")
                );

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.PreferredLanguage == "en" && x.Year == 1 &&
                x.LastPage == 0 && x.Stage == Stage.GatheredYear);
            
            confirmHandler.Verify(x => x.ShowInstruction(
                    It.Is<IncompleteUser>(y => y.Id == "sample-registering-user-with-lang"), 0), Times.Once);
            confirmHandler.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldShowListPageCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IGatherClassMessageHandler>();

            var handler = new GatherYearMessageHandler(mediator.Object, client.Object,
                MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context,
                MockBuilder.BuildFakeLogger<GatherYearMessageHandler>(), MockBuilder.BuildFakeCultures());
            await handler.Handle(
                await context.IncompleteUsers.FindAsync("sample-registering-user-with-lang"),
                new Payload(PayloadType.Year, 1)
                );
                        
            var expectedMessage = new SendRequest("sample-registering-user-with-lang", new Message("year-selection-text", new[] 
            {
                new QuickReply("11", new Payload(PayloadType.Year, "11").ToJson()),
                new QuickReply("12", new Payload(PayloadType.Year, "12").ToJson()),
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson()),
                new QuickReply("<-", new Payload(PayloadType.Year, 0).ToJson())
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
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IGatherClassMessageHandler>();

            var handler = new GatherYearMessageHandler(mediator.Object, client.Object,
                MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context,
                MockBuilder.BuildFakeLogger<GatherYearMessageHandler>(), MockBuilder.BuildFakeCultures());
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user-with-lang"));
            
            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.PreferredLanguage == "en" &&
                x.LastPage == 0 && x.Stage == Stage.GatheredLanguage);
            
            var expectedMessage = new SendRequest("sample-registering-user-with-lang", new Message("year-selection-text", new[] 
            {
                new QuickReply("1", new Payload(PayloadType.Year, "1").ToJson()),
                new QuickReply("2", new Payload(PayloadType.Year, "2").ToJson()),
                new QuickReply("3", new Payload(PayloadType.Year, "3").ToJson()),
                new QuickReply("4", new Payload(PayloadType.Year, "4").ToJson()),
                new QuickReply("5", new Payload(PayloadType.Year, "5").ToJson()),
                new QuickReply("6", new Payload(PayloadType.Year, "6").ToJson()),
                new QuickReply("7", new Payload(PayloadType.Year, "7").ToJson()),
                new QuickReply("8", new Payload(PayloadType.Year, "8").ToJson()),
                new QuickReply("9", new Payload(PayloadType.Year, "9").ToJson()),
                new QuickReply("10", new Payload(PayloadType.Year, "10").ToJson()),
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson()),
                new QuickReply("->", new Payload(PayloadType.Year, 1).ToJson())
                
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
            var mediator = MockBuilder.BuildMediatorMock();
            var client = new Mock<ISendApiClient>();
            var confirmHandler = new Mock<IGatherClassMessageHandler>();

            var handler = new GatherYearMessageHandler(mediator.Object, client.Object,
                MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context,
                MockBuilder.BuildFakeLogger<GatherYearMessageHandler>(), MockBuilder.BuildFakeCultures());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-lang"), new Payload());
            
            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.PreferredLanguage == "en" &&
                x.LastPage == 0 && x.Stage == Stage.GatheredLanguage);

            var expectedMessage = new SendRequest("sample-registering-user-with-lang", new Message("unsupported-command-text", new[] 
            {
                new QuickReply("1", new Payload(PayloadType.Year, "1").ToJson()),
                new QuickReply("2", new Payload(PayloadType.Year, "2").ToJson()),
                new QuickReply("3", new Payload(PayloadType.Year, "3").ToJson()),
                new QuickReply("4", new Payload(PayloadType.Year, "4").ToJson()),
                new QuickReply("5", new Payload(PayloadType.Year, "5").ToJson()),
                new QuickReply("6", new Payload(PayloadType.Year, "6").ToJson()),
                new QuickReply("7", new Payload(PayloadType.Year, "7").ToJson()),
                new QuickReply("8", new Payload(PayloadType.Year, "8").ToJson()),
                new QuickReply("9", new Payload(PayloadType.Year, "9").ToJson()),
                new QuickReply("10", new Payload(PayloadType.Year, "10").ToJson()),
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson()),
                new QuickReply("->", new Payload(PayloadType.Year, 1).ToJson())
            }));
            
            client.Verify(x => x.Send(
                It.Is<SendRequest>(y => y.IsEquivalentTo(expectedMessage))
                ));
            client.VerifyNoOtherCalls();
        }
    }
}