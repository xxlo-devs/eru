using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand
{
    public class UnsupportedCommandMessageHandler : IUnsupportedCommandMessageHandler
    {
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IMediator _mediator;
        private readonly ISelector _selector;
        
        public UnsupportedCommandMessageHandler(ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, IMediator mediator, ISelector selector)
        {
            _apiClient = apiClient;
            _translator = translator;
            _mediator = mediator;
            _selector = selector;
        }
        
        public async Task Handle(string uid)
        {
            var user = await _mediator.Send(new GetSubscriberQuery(uid, "FacebookMessenger"));
            var response = new SendRequest(uid,
                new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), 
                    await _selector.GetCancelSelector(user.PreferredLanguage)));
            await _apiClient.Send(response);
        }
    }
}