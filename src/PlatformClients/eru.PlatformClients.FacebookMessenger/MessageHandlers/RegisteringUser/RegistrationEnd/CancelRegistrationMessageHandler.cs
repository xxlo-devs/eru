using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Message = eru.PlatformClients.FacebookMessenger.Models.SendApi.Message;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd
{
    public class CancelRegistrationMessageHandler : MessageHandler<CancelRegistrationMessageHandler>
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;

        public CancelRegistrationMessageHandler(IServiceProvider provider, ILogger<CancelRegistrationMessageHandler> logger) : base(logger)
        {
            _dbContext = provider.GetService<IRegistrationDbContext>();
            _apiClient = provider.GetService<ISendApiClient>();
            _translator = provider.GetService<ITranslator<FacebookMessengerPlatformClient>>();
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