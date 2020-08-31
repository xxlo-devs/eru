using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Properties;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserTests
{
    public class CancelSubscriptionHandlerTests
    {
        private Mock<IMediator> BuildFakeMediator()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>()))
                .Returns((GetSubscriberQuery query, CancellationToken cancellationToken) =>
                {
                    if (query.Id == "sample-subscriber-id" && query.Platform == "FacebookMessenger")
                    {
                        return Task.FromResult(new Subscriber
                        {
                            Id = "sample-subscriber-id",
                            Platform = "FacebookMessenger",
                            Class = "sample-class",
                            PreferredLanguage = "en-us"
                        });
                    }
                    else
                    {
                        return Task.FromResult<Subscriber>(null);
                    }
                });

            mediator.Setup(x => x.Send(It.IsAny<CancelSubscriptionCommand>(), It.IsAny<CancellationToken>())).Returns(
                (CancelSubscriptionCommand command, CancellationToken cancellationToken) =>
                {
                    if (command.Id != "sample-subscriber-id" || command.Platform != "FacebookMessenger")
                        throw new Exception();
                    else return Task.FromResult<Unit>(Unit.Value);
                });

            return mediator;
        }
        
        [Fact]
        public async void ShouldCancelSubscription()
        {
            var mediator = BuildFakeMediator();
            var apiClient = new Mock<ISendApiClient>();

            var handler = new CancelSubscriptionMessageHandler(mediator.Object, apiClient.Object);
            await handler.Handle("sample-subscriber-id");
            
            mediator.Verify(x => x.Send(It.IsAny<CancelSubscriptionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}