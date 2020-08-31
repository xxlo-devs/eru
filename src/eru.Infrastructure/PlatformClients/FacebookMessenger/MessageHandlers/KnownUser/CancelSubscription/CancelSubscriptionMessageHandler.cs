using System.Threading.Tasks;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription
{
    public class CancelSubscriptionMessageHandler : ICancelSubscriptionMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        public CancelSubscriptionMessageHandler(IMediator mediator, ISendApiClient apiClient)
        {
            _mediator = mediator;
            _apiClient = apiClient;
        }
        
        public async Task Handle(string uid)
        {
            await _mediator.Send(new CancelSubscriptionCommand(uid, "FacebookMessenger"));
            var response = new SendRequest(uid, new Message("We are sorry to see you go. Your subscription (and your data) has been deleted. If you will ever want to subscribe again, write anything to start the process."));
            await _apiClient.Send(response);
        }
    }
}