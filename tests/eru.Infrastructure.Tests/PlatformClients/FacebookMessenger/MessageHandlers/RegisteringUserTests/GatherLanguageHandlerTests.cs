using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    public class GatherLanguageHandlerTests
    {
        [Fact]
        public async void ShoudGatherLanguageCorrectly()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new GatherLanguageMessageHandler();
            await handler.Handle("sample-registering-user", "language:en");

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user" && x.Platform == "FacebookMessenger" && x.PreferredLanguage == "en" &&
                x.Stage == Stage.GatheredLanguage);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
    }
}