using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
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
            var mediator = new Mock<IMediator>();
            var client = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            var confirmHandler = new Mock<IGatherClassMessageHandler>();

            var handler = new GatherYearMessageHandler(mediator.Object, client.Object, translator.Object, confirmHandler.Object, context, new Mock<ILogger<GatherYearMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user-with-lang"), new Payload(PayloadType.Year, "1"));

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.PreferredLanguage == "en" && x.Year == 1 &&
                x.LastPage == 0 && x.Stage == Stage.GatheredYear);
        }

        [Fact]
        public async void ShouldShowInstructionCorrectly()
        {
            
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            
        }
    }
}