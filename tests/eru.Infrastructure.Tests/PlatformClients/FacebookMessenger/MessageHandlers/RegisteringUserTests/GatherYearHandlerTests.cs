using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    public class GatherYearHandlerTests
    {
        [Fact]
        public async void ShouldGatherYearCorrectly()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new GatherYearMessageHandler();
            await handler.Handle("sample-registering-user-with-lang", "year:1");

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.Platform == "FacebookMessenger" && x.Year == 1 &&
                x.PreferredLanguage == "en-us" && x.Stage == Stage.GatheredYear);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
    }
}