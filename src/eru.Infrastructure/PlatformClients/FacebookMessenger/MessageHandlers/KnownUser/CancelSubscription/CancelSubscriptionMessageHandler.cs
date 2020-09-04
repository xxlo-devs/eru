using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription
{
    public class CancelSubscriptionMessageHandler : ICancelSubscriptionMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        public CancelSubscriptionMessageHandler(IMediator mediator, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _translator = translator;
        }
        
        public async Task Handle(string uid)
        {
            var user = await _mediator.Send(new GetSubscriberQuery(uid, "FacebookMessenger"));
            await _mediator.Send(new CancelSubscriptionCommand(uid, "FacebookMessenger"));
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("subscription-cancelled", user.PreferredLanguage)));
            await _apiClient.Send(response);
        }
    }
}