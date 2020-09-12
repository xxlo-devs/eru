﻿using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Static;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Message = eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Message;
using QuickReply = eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties.QuickReply;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.KnownUser
{
    public class CancelSubscriptionMessageHandlerTests
    {
        [Fact]
        public async void ShouldCancelSubscriptionCorrectly()
        {
            var mediator = BuildMediatorMock();
            var apiClient = new Mock<ISendApiClient>();

            var message = new Messaging
            {
                Sender = new Sender {Id = "sample-subscriber"},
                Recipient = new Recipient {Id = "sample-page-id"},
                Timestamp = 123456789,
                Message = new Message
                {
                    Mid = "sample-message-id",
                    Text = "Cancel",
                    QuickReply = new QuickReply {Payload = new Payload(PayloadType.Cancel).ToJson()}
                }
            };
            
            var handler = new CancelSubscriptionMessageHandler(mediator.Object, apiClient.Object, BuildFakeTranslator(), new Mock<ILogger<CancelSubscriptionMessageHandler>>().Object);
            await handler.Handle(message);

            mediator.Verify(x => x.Send(It.Is<GetSubscriberQuery>(y => y.Id == "sample-subscriber" && y.Platform == FacebookMessengerPlatformClient.PId), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(x => x.Send(It.Is<CancelSubscriptionCommand>(y => y.Id == "sample-subscriber" && y.Platform == FacebookMessengerPlatformClient.PId), It.IsAny<CancellationToken>()), Times.Once);
            mediator.VerifyNoOtherCalls();
            
            apiClient.Verify(x => x.Send(It.Is<SendRequest>(
                y => y.Type == MessagingTypes.Response 
                     && y.Recipient.Id == "sample-subscriber"
                     && y.Message.Text == "subscription-cancelled-text"
                     && y.Message.QuickReplies == null
            )));
            apiClient.VerifyNoOtherCalls();
        }

        private Mock<IMediator> BuildMediatorMock()
        {
            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>())).Returns(
                (GetSubscriberQuery query, CancellationToken cancellationToken) =>
                {
                    if (query.Id == "sample-subscriber" && query.Platform == FacebookMessengerPlatformClient.PId)
                        return Task.FromResult(new Subscriber
                        {
                            Id = "sample-subscriber", Platform = FacebookMessengerPlatformClient.PId,
                            PreferredLanguage = "en", Class = "sample-class"
                        });
                    else
                        return Task.FromResult<Subscriber>(null);
                });

            return mediator;
        }
        
        private ITranslator<FacebookMessengerPlatformClient> BuildFakeTranslator()
        {
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            translator.Setup(x => x.TranslateString("subscription-cancelled", "en")).Returns(Task.FromResult("subscription-cancelled-text"));
            return translator.Object;
        }
    }
}