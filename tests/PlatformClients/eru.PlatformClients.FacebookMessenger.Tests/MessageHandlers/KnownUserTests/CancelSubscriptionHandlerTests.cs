using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.KnownUserTests
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
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            translator.Setup(x => x.TranslateString("subscription-cancelled", "en")).Returns(Task.FromResult("We are sorry to see you go. Your subscription (and your data) has been deleted. If you will ever want to subscribe again, write anything to start the registration process."));
            var apiClient = new Mock<ISendApiClient>();
            var logger = new Mock<ILogger<CancelSubscriptionMessageHandler>>();
            
            var handler = new CancelSubscriptionMessageHandler(mediator.Object, apiClient.Object, translator.Object, logger.Object);
            await handler.Handle("sample-subscriber-id");
            
            mediator.Verify(x => x.Send(It.IsAny<CancelSubscriptionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}