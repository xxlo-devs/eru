using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
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
    public class GatherClassMessageHandlerTests
    {
        [Fact]
        public async void ShouldUpdateUserCorrectly()
        {
            var context = new FakeRegistrationDb();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(MockBuilder.BuildMediatorMock().Object, new Mock<ISendApiClient>().Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherClassMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"),
                new Payload(PayloadType.Class, "sample-class-1a"));

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-year" && x.PreferredLanguage == "en" && x.Year == 1 &&
                x.ClassId == "sample-class-1a" && x.LastPage == 0 && x.Stage == Stage.GatheredClass);
            
            confirmHandler.Verify(
                x => x.ShowInstruction(It.Is<IncompleteUser>(y => y.Id == "sample-registering-user-with-year")),
                Times.Once);
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
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"),
                new Payload(PayloadType.Class, 1));
            
            var expectedMessage = new SendRequest("sample-registering-user-with-year", new Message("class-selection-text", new[] 
            {
                new QuickReply("1k", new Payload(PayloadType.Class, "sample-class-1k").ToJson()),
                new QuickReply("1l", new Payload(PayloadType.Class, "sample-class-1l").ToJson()),
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson()),
                new QuickReply("<-", new Payload(PayloadType.Class, 0).ToJson())
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
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherClassMessageHandler>());
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"));

            var expectedMessage = new SendRequest("sample-registering-user-with-year", new Message("class-selection-text", new[]
            {
                new QuickReply("1a", new Payload(PayloadType.Class, "sample-class-1a").ToJson()),
                new QuickReply("1b", new Payload(PayloadType.Class, "sample-class-1b").ToJson()),
                new QuickReply("1c", new Payload(PayloadType.Class, "sample-class-1c").ToJson()),
                new QuickReply("1d", new Payload(PayloadType.Class, "sample-class-1d").ToJson()),
                new QuickReply("1e", new Payload(PayloadType.Class, "sample-class-1e").ToJson()),
                new QuickReply("1f", new Payload(PayloadType.Class, "sample-class-1f").ToJson()),
                new QuickReply("1g", new Payload(PayloadType.Class, "sample-class-1g").ToJson()),
                new QuickReply("1h", new Payload(PayloadType.Class, "sample-class-1h").ToJson()),
                new QuickReply("1i", new Payload(PayloadType.Class, "sample-class-1i").ToJson()),
                new QuickReply("1j", new Payload(PayloadType.Class, "sample-class-1j").ToJson()),
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson()),
                new QuickReply("->", new Payload(PayloadType.Class, 1).ToJson())
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
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, MockBuilder.BuildFakeTranslator(), confirmHandler.Object, context, MockBuilder.BuildFakeLogger<GatherClassMessageHandler>());
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"), new Payload());
            
            var expectedMessage = new SendRequest("sample-registering-user-with-year", new Message("unsupported-command-text", new[]
            {
                new QuickReply("1a", new Payload(PayloadType.Class, "sample-class-1a").ToJson()),
                new QuickReply("1b", new Payload(PayloadType.Class, "sample-class-1b").ToJson()),
                new QuickReply("1c", new Payload(PayloadType.Class, "sample-class-1c").ToJson()),
                new QuickReply("1d", new Payload(PayloadType.Class, "sample-class-1d").ToJson()),
                new QuickReply("1e", new Payload(PayloadType.Class, "sample-class-1e").ToJson()),
                new QuickReply("1f", new Payload(PayloadType.Class, "sample-class-1f").ToJson()),
                new QuickReply("1g", new Payload(PayloadType.Class, "sample-class-1g").ToJson()),
                new QuickReply("1h", new Payload(PayloadType.Class, "sample-class-1h").ToJson()),
                new QuickReply("1i", new Payload(PayloadType.Class, "sample-class-1i").ToJson()),
                new QuickReply("1j", new Payload(PayloadType.Class, "sample-class-1j").ToJson()),
                new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson()),
                new QuickReply("->", new Payload(PayloadType.Class, 1).ToJson())
            }));
            
            client.Verify(x => x.Send(It.Is<SendRequest>(
                    y => y.IsEquivalentTo(expectedMessage))
                ));
            client.VerifyNoOtherCalls();
        }
    }
}