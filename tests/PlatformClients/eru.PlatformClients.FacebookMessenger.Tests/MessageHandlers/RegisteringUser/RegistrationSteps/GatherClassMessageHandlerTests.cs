using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
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
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, translator.Object, confirmHandler.Object, context, new Mock<ILogger<GatherClassMessageHandler>>().Object);
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
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, translator.Object, confirmHandler.Object, context, new Mock<ILogger<GatherClassMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"), new Payload(PayloadType.Class, 1));
        }

        [Fact]
        public async void ShouldShowInstructionCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, translator.Object, confirmHandler.Object, context, new Mock<ILogger<GatherClassMessageHandler>>().Object);
            await handler.ShowInstruction(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"), 1);
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            var context = new FakeRegistrationDb();
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            var confirmHandler = new Mock<IConfirmSubscriptionMessageHandler>();

            var handler = new GatherClassMessageHandler(mediator.Object, client.Object, translator.Object, confirmHandler.Object, context, new Mock<ILogger<GatherClassMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-year"), new Payload());
        }
    }
}