using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
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
            SetupMediator();
            SetupTranslator();
            SetupApiClient();
            SetupLogger();
            
            PlatformClient = new FacebookMessengerPlatformClient(ApiClientMock.Object, MediatorMock.Object, TranslatorMock.Object, LoggerMock.Object);
        }

        private void SetupMediator()
        {
            MediatorMock = new Mock<IMediator>();
            MediatorMock.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>())).Returns(
                (GetSubscriberQuery query, CancellationToken cancellationToken) =>
                {
                    if (query.Id == "sample-subscriber" && query.Platform == FacebookMessengerPlatformClient.PId)
                    {
                        return Task.FromResult(new Subscriber { Id = "sample-subscriber", Platform = FacebookMessengerPlatformClient.PId, PreferredLanguage = "en", Class = "sample-class" });
                    }
                    else
                    {
                        throw new Exception();
                    }
                });
        }

        private void SetupTranslator()
        {
            TranslatorMock = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            TranslatorMock.Setup(x => x.TranslateString("closing-substitutions", "en")).Returns(Task.FromResult("closing-substitution-text"));
            TranslatorMock.Setup(x => x.TranslateString("new-substitutions", "en")).Returns(Task.FromResult("new-substitutions-text"));
            TranslatorMock.Setup(x => x.TranslateString("substitution", "en")).Returns(Task.FromResult("SUBSTITUTION | {0} | {1} | {2} | {3} | {4} | {5}"));
            TranslatorMock.Setup(x => x.TranslateString("cancellation", "en")).Returns(Task.FromResult("CANCELLATION | {0} | {1} | {2} | {3} | {4}"));
            TranslatorMock.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("cancel-button-text"));
        }

        private void SetupApiClient()
        {
            ApiClientMock = new Mock<ISendApiClient>();
        }

        private void SetupLogger()
        {
            LoggerMock = new Mock<ILogger<FacebookMessengerPlatformClient>>();
        }
        
        public FacebookMessengerPlatformClient PlatformClient { get; set; }
        public Mock<IMediator> MediatorMock { get; set; }
        public Mock<ISendApiClient> ApiClientMock { get; set; }
        public Mock<ITranslator<FacebookMessengerPlatformClient>> TranslatorMock { get; set; }
        public Mock<ILogger<FacebookMessengerPlatformClient>> LoggerMock { get; set; }
    }
}