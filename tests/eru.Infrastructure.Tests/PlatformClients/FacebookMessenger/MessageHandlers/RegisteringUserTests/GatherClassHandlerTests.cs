using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    public class GatherClassHandlerTests
    {
        [Fact]
        public async void ShouldGatherClassCorrectly()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new GatherClassMessageHandler();
            await handler.Handle("sample-registering-user-with-year", "class:sample-class");

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-year" && x.Platform == "FacebookMessenger" && x.Year == 1 &&
                x.ClassId == "sample-class" && x.PreferredLanguage == "en-us" && x.Stage == Stage.GatheredClass);
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
    }
}