using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Message = eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Message;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.CancelRegistration
{
    public class CancelRegistrationMessageHandler : MessageHandler<CancelRegistrationMessageHandler>, ICancelRegistrationMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;

        public CancelRegistrationMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, ILogger<CancelRegistrationMessageHandler> logger) : base(logger)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _translator = translator;
        }

        protected override async Task Base(Messaging message)
        {
            var uid = message.Sender.Id;
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            _dbContext.IncompleteUsers.Remove(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("subscription-cancelled", user.PreferredLanguage)));
            await _apiClient.Send(response);
        }
    }
}