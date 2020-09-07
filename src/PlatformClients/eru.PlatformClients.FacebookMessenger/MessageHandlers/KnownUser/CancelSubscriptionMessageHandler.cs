using System;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Message = eru.PlatformClients.FacebookMessenger.Models.SendApi.Message;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription
{
    public class CancelSubscriptionMessageHandler : MessageHandler<CancelSubscriptionMessageHandler>
    {
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        
        public CancelSubscriptionMessageHandler(IServiceProvider provider, ILogger<CancelSubscriptionMessageHandler> logger) : base(logger)
        {
            _mediator = provider.GetService<IMediator>();
            _translator = provider.GetService<ITranslator<FacebookMessengerPlatformClient>>();
            _apiClient = provider.GetService<ISendApiClient>();
        }

        protected override async Task Base(Messaging message)
        {
            var uid = message.Sender.Id;
            
            var user = await _mediator.Send(new GetSubscriberQuery(uid, FacebookMessengerPlatformClient.PId));
            await _mediator.Send(new CancelSubscriptionCommand(uid, FacebookMessengerPlatformClient.PId));
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("subscription-cancelled", user.PreferredLanguage)));
            await _apiClient.Send(response);
        }
    }
}