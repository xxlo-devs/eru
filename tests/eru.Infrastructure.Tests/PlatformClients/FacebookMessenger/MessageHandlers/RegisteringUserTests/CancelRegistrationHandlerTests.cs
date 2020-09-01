using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    public class CancelRegistrationHandlerTests
    {
        [Fact]
        public async void ShouldCancelRegistrationCorrectly()
        {
            var context = new FakeRegistrationDb();
            var apiClient = new Mock<ISendApiClient>();
            
            var handler = new CancelRegistrationMessageHandler(context, apiClient.Object);
            await handler.Handle("sample-registering-user-with-class");

            context.IncompleteUsers.Should().NotContain(x => x.Id == "sample-registering-user-with-class");
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
    }
}