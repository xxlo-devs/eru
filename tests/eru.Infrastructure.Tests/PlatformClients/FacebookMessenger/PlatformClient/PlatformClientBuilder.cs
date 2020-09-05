using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Moq;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.PlatformClient
{
    internal class PlatformClientBuilder
    {
        public PlatformClientBuilder()
        {
            SetupMediator();
            SetupSelector();
            SetupTranslator();
            SetupApiClient();
            
            PlatformClient = new FacebookMessengerPlatformClient(ApiClientMock.Object, MediatorMock.Object, SelectorMock.Object, TranslatorMock.Object);
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

        private void SetupSelector()
        {
            SelectorMock = new Mock<ISelector>(); 
            SelectorMock.Setup(x => x.GetCancelSelector("en")).Returns(Task.FromResult(new[] {new QuickReply("Cancel", new Payload(PayloadType.Cancel).ToJson())}.AsEnumerable()));
        }

        private void SetupTranslator()
        {
            TranslatorMock = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            TranslatorMock.Setup(x => x.TranslateString("closing-substitutions", "en")).Returns(Task.FromResult("If you want to stop getting these notifications, just click Cancel."));
            TranslatorMock.Setup(x => x.TranslateString("new-substitutions", "en")).Returns(Task.FromResult("Here are substitutions for the next day!"));
            TranslatorMock.Setup(x => x.TranslateString("substitution", "en")).Returns(Task.FromResult("Teacher {0} on {1} lesson (course: {2}) will be substituted by teacher {3} in {4} room. School notes: {5}."));
            TranslatorMock.Setup(x => x.TranslateString("cancellation", "en")).Returns(Task.FromResult("Lesson {0} (subject: {1}, teacher: {2}, room: {3}) is cancelled. School note: {4}."));
            TranslatorMock.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("Cancel"));
        }

        private void SetupApiClient()
        {
            ApiClientMock = new Mock<ISendApiClient>();
        }
        
        public FacebookMessengerPlatformClient PlatformClient { get; set; }
        public Mock<IMediator> MediatorMock { get; set; }
        public Mock<ISendApiClient> ApiClientMock { get; set; }
        public Mock<ISelector> SelectorMock { get; set; }
        public Mock<ITranslator<FacebookMessengerPlatformClient>> TranslatorMock { get; set; }
    }
}