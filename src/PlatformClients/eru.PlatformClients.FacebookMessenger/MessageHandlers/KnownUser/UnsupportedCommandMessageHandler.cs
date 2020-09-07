using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand
{
    public class UnsupportedCommandMessageHandler : MessageHandler<UnsupportedCommandMessageHandler>
    {
        /*public UnsupportedCommandMessageHandler(ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, IMediator mediator, ISelector selector, ILogger<UnsupportedCommandMessageHandler> logger)
        {
            _apiClient = apiClient;
            _translator = translator;
            _mediator = mediator;
            _selector = selector;
            _logger = logger;
        }
        
        public async Task Handle(string uid)
        {
            _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: UnsupportedCommandMessageHandler.UnsupportedCommand got a request (uid: {uid})");
            var user = await _mediator.Send(new GetSubscriberQuery(uid, FacebookMessengerPlatformClient.PId));
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), await _selector.GetCancelSelector(user.PreferredLanguage)));
            await _apiClient.Send(response);
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: UnsupportedCommandMessageHandler.UnsupportedCommand has processed a request from user (uid: {uid})");

        }*/
        public UnsupportedCommandMessageHandler(ILogger<UnsupportedCommandMessageHandler> logger) : base(logger)
        {
        }

        protected override async Task Base(Messaging message)
        {
            throw new System.NotImplementedException();
        }
    }
}