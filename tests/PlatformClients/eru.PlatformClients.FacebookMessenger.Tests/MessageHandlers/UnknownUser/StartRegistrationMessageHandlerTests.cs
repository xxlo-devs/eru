using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using FluentAssertions;
using Hangfire;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.UnknownUser
{
    public class StartRegistrationMessageHandlerTests
    {
        [Fact]
        public async void ShouldStartRegistrationCorrectly()
        {
            var context = new FakeRegistrationDb();
            var langHandlerMock = new Mock<IGatherLanguageMessageHandler>();
            var backgroundJobClientMock = new Mock<IBackgroundJobClient>();
            
            var message = new Messaging
            {
                Sender = new Sender{Id = "sample-unknown-user"},
                Recipient = new Recipient{Id = "sample-page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "sample-message-text"
                }
            };
            
            var handler = new StartRegistrationMessageHandler(context,
                langHandlerMock.Object, backgroundJobClientMock.Object,
                MockBuilder.BuildFakeLogger<StartRegistrationMessageHandler>(), MockBuilder.BuildFakeCultures());
            await handler.Handle(message);

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-unknown-user" && x.PreferredLanguage == "en" && x.Year == 0 && x.ClassId == null &&
                x.Stage == Stage.Created && x.LastPage == 0);
            
            var user = await context.IncompleteUsers.FindAsync("sample-unknown-user");
            langHandlerMock.Verify(x => x.ShowInstruction(user, 0));
            langHandlerMock.VerifyNoOtherCalls();
        }
    }
}