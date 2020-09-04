using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand
{
    public class UnsupportedCommandMessageHandler : IUnsupportedCommandMessageHandler
    {
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IMediator _mediator; 
        
        public UnsupportedCommandMessageHandler(ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, IMediator mediator)
        {
            _apiClient = apiClient;
            _translator = translator;
            _mediator = mediator;
        }
        
        public async Task Handle(string uid)
        {
            var user = await _mediator.Send(new GetSubscriberQuery(uid, "FacebookMessenger"));
            await _apiClient.Send(new SendRequest(uid,
                new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), new[]
                {
                    new QuickReply("Cancel", new Payload(Type.Cancel).ToJson()), 
                })));
        }
    }
}