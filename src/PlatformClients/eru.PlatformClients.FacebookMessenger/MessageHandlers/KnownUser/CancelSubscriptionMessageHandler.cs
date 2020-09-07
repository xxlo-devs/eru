using System;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription
{
    public class CancelSubscriptionMessageHandler : MessageHandler<CancelSubscriptionMessageHandler>
    {
        /*public CancelSubscriptionMessageHandler(IMediator mediator, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, ILogger<CancelSubscriptionMessageHandler> logger)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _translator = translator;
            _logger = logger;
        }
        
        public async Task Handle(string uid)
        {
            _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: CancelSubscriptionMessageHandler.Handle got a request (uid: {uid})");
            var user = await _mediator.Send(new GetSubscriberQuery(uid, FacebookMessengerPlatformClient.PId));
            await _mediator.Send(new CancelSubscriptionCommand(uid, FacebookMessengerPlatformClient.PId));
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("subscription-cancelled", user.PreferredLanguage)));
            await _apiClient.Send(response);
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: CancelSubscriptionMessageHandler.Handle has successfully cancelled user (uid: {uid}) subsciription");
        }*/
        public CancelSubscriptionMessageHandler(IServiceProvider provider, ILogger<CancelSubscriptionMessageHandler> logger) : base(logger)
        {
        }

        protected override async Task Base(Messaging message)
        {
            throw new System.NotImplementedException();
        }
    }
}