using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Moq;

namespace eru.PlatformClients.FacebookMessenger.Tests.PlatformClient
{
    internal class PlatformClientBuilder
    {
        public PlatformClientBuilder()
        {
            ApiClientMock = new Mock<ISendApiClient>();

            PlatformClient = new FacebookMessengerPlatformClient(ApiClientMock.Object,
                MockBuilder.BuildMediatorMock().Object, MockBuilder.BuildFakeTranslator(),
                MockBuilder.BuildFakeLogger<FacebookMessengerPlatformClient>());
        }
        
        public FacebookMessengerPlatformClient PlatformClient { get; set; }
        public Mock<ISendApiClient> ApiClientMock { get; set; }
    }
}