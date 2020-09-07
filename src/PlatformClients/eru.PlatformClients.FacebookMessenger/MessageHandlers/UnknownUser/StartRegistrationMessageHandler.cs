using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Hangfire.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Message = eru.PlatformClients.FacebookMessenger.Models.SendApi.Message;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public class StartRegistrationMessageHandler : MessageHandler<StartRegistrationMessageHandler>
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _client;
        private readonly IConfiguration _configuration;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly RegistrationMessageHandler<GatherLanguageMessageHandler> _langHandler;
        
        public StartRegistrationMessageHandler(IServiceProvider provider, ILogger<StartRegistrationMessageHandler> logger) : base(logger)
        {
            _dbContext = provider.GetService<IRegistrationDbContext>();
            _client = provider.GetService<ISendApiClient>();
            _configuration = provider.GetService<IConfiguration>();
            _translator = provider.GetService<ITranslator<FacebookMessengerPlatformClient>>();
            _langHandler = provider.GetService<RegistrationMessageHandler<GatherLanguageMessageHandler>>();
        }

        protected override async Task Base(Messaging message)
        {
            var incompleteUser = new IncompleteUser(message.Sender.Id);

            await _dbContext.IncompleteUsers.AddAsync(incompleteUser);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            await _langHandler.ShowInstruction(incompleteUser, 0);
        }
    }
}