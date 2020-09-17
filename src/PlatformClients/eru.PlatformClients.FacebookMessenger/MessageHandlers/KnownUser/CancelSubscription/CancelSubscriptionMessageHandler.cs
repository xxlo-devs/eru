using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using MediatR;
using Microsoft.Extensions.Logging;
using Message = eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Message;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription
{
    public class CancelSubscriptionMessageHandler : MessageHandler<CancelSubscriptionMessageHandler>,
        ICancelSubscriptionMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IApplicationCultures _cultures;
        
        public CancelSubscriptionMessageHandler(IMediator mediator, ISendApiClient apiClient,
            ITranslator<FacebookMessengerPlatformClient> translator,
            ILogger<CancelSubscriptionMessageHandler> logger, IApplicationCultures cultures)
            : base(logger)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _translator = translator;
            _cultures = cultures;
        }

        protected override async Task Base(Messaging message)
        {
            var uid = message.Sender.Id;
            
            var user = await _mediator.Send(new GetSubscriberQuery(uid, FacebookMessengerPlatformClient.PId));
            await _mediator.Send(new CancelSubscriptionCommand(uid, FacebookMessengerPlatformClient.PId));
            
            await _apiClient.Send(new SendRequest(uid, 
                new Message(await _translator.TranslateString("subscription-cancelled", _cultures.FindCulture(user.PreferredLanguage)))));
        }
    }
}