using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription
{
    public class ConfirmSubscriptionMessageHandler : IConfirmSubscriptionMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ISelector _selector;
        private readonly ILogger<ConfirmSubscriptionMessageHandler> _logger;

        public ConfirmSubscriptionMessageHandler(IMediator mediator, IRegistrationDbContext dbContext, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, ISelector selector, ILogger<ConfirmSubscriptionMessageHandler> logger)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _dbContext = dbContext;
            _translator = translator;
            _selector = selector;
            _logger = logger;
        }

        public async Task Handle(string uid, Payload payload)
        {
            _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: ConfirmSubscriptionMessageHandler.Handle got a request (uid: {uid}, payload: {payload.ToJson()})");
            
            if (payload.Type == PayloadType.Subscribe)
            {
                _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: ConfirmSubscriptionMessageHandler.Handle redirected request to ConfirmSubscriptionMessageHandler.Confirm");
                await Confirm(uid);
            }
            else
            {
                _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: ConfirmSubscriptionMessageHandler.Handle redirected request to ConfirmSubscriptionMessageHandler.UnsupportedCommand");
                await UnsupportedCommand(uid);
            }
        }

        private async Task Confirm(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            await _mediator.Send(user.ToCreateSubscriptionCommand());
            
            _dbContext.IncompleteUsers.Remove(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("congratulations", user.PreferredLanguage), await _selector.GetCancelSelector(user.PreferredLanguage)));
            await _apiClient.Send(response);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: ConfirmSubscriptionMessageHandler.Confirm has successfully created a subscription for user (uid: {uid})");
        }

        private async Task UnsupportedCommand(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), await _selector.GetConfirmationSelector(user.PreferredLanguage)));
            await _apiClient.Send(response);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: ConfirmSubscriptionMessageHandler.UnsupportedCommand has processed request from user (uid: {uid})");
        }
    }
}