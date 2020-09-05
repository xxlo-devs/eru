using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Commands.CancelSubscription;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription
{
    public class CancelSubscriptionMessageHandler : ICancelSubscriptionMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ILogger _logger;
        public CancelSubscriptionMessageHandler(IMediator mediator, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, ILogger logger)
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
        }
    }
}