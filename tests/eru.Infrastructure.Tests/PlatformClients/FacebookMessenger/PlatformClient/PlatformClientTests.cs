using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.PlatformClient
{
    public class PlatformClientTests
    {
        [Fact]
        public async void ShouldSendGenericMessageCorrectly()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>())).Returns(
                (GetSubscriberQuery query, CancellationToken cancellationToken) =>
                {
                    return Task.FromResult(new Subscriber
                    {
                        Id = "sample-subscriber",
                        Platform = "FacebookMessenger",
                        Class = "sample-class",
                        PreferredLanguage = "en"
                    });
                });
            
            var apiClient = new Mock<ISendApiClient>();
            var platformClient = new FacebookMessengerPlatformClient(apiClient.Object, mediator.Object);

            await platformClient.SendMessage("sample-subscriber", "sample message");
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }

        [Fact]
        public async void ShouldSendSubstitutionsCorrectly()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>())).Returns(
                (GetSubscriberQuery query, CancellationToken cancellationToken) =>
                {
                    return Task.FromResult(new Subscriber
                    {
                        Id = "sample-subscriber",
                        Platform = "FacebookMessenger",
                        Class = "sample-class",
                        PreferredLanguage = "en"
                    });
                });
            
            var apiClient = new Mock<ISendApiClient>();
            var platformClient = new FacebookMessengerPlatformClient(apiClient.Object, mediator.Object);

            await platformClient.SendMessage("sample-subscriber", new []
            {
                new Substitution{Teacher = "sample-teacher", Lesson = 2, Subject = "sample-subject", Classes = new[] {new Class(1, "sample-section")}, Groups = "sample-group", Cancelled = true, Substituting = "cancelled", Note = "sample-note", Room = "sample-room"}, 
                new Substitution{Teacher = "sample-teacher-2", Lesson = 3, Subject = "sample-subject-2", Classes = new[] {new Class(2, "sample-section-2")}, Groups = "sample-group-2", Cancelled = false, Substituting = "sample-substituting-teacher", Note = "sample-note-2", Room = "sample-room-2"}, 
            });
            
            apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Exactly(4));
        }
    }
}