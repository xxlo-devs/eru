using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace eru.PlatformClients.FacebookMessenger.Tests.PlatformClient
{
    internal class PlatformClientBuilder
    {
        public PlatformClientBuilder()
        {
            MediatorMock = MockBuilder.BuildMediatorMock();
            ApiClientMock = new Mock<ISendApiClient>();
            FakeLogger = MockBuilder.BuildFakeLogger<FacebookMessengerPlatformClient>();

            PlatformClient = new FacebookMessengerPlatformClient(ApiClientMock.Object, MediatorMock.Object, MockBuilder.BuildFakeTranslator(), FakeLogger);
        }
        
        public FacebookMessengerPlatformClient PlatformClient { get; set; }
        public Mock<ISendApiClient> ApiClientMock { get; set; }
        private Mock<IMediator> MediatorMock { get; set; }
        private ILogger<FacebookMessengerPlatformClient> FakeLogger { get; set; }
    }
}