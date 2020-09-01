using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserTests
{
    public class UnsupportedCommandHandlerTests
    {
        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectlyOnCreatedStage()
        {
            var apiClient = new Mock<ISendApiClient>();
            var handler = new UnsupportedCommandMessageHandler(apiClient.Object);
            
            await handler.Handle("sample-subscriber-id");
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectlyOnGatheredLanguageStage()
        {
            var apiClient = new Mock<ISendApiClient>();
            var handler = new UnsupportedCommandMessageHandler(apiClient.Object);
            
            await handler.Handle("sample-subscriber-id");
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectlyOnGatheredYearStage()
        {
            var apiClient = new Mock<ISendApiClient>();
            var handler = new UnsupportedCommandMessageHandler(apiClient.Object);
            
            await handler.Handle("sample-subscriber-id");
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectlyOnGatheredClassStage()
        {
            var apiClient = new Mock<ISendApiClient>();
            var handler = new UnsupportedCommandMessageHandler(apiClient.Object);
            
            await handler.Handle("sample-subscriber-id");
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
    }
}