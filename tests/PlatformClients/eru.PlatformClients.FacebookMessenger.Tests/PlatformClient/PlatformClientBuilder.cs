using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
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
        public Mock<IMediator> MediatorMock { get; set; }
        public Mock<ISendApiClient> ApiClientMock { get; set; }
        public ILogger<FacebookMessengerPlatformClient> FakeLogger { get; set; }
    }
}