using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public class StartRegistrationMessageHandler : MessageHandler<StartRegistrationMessageHandler>,
        IUnknownUserMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly IGatherLanguageMessageHandler _langHandler;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IApplicationCultures _cultures;
        
        public StartRegistrationMessageHandler(IRegistrationDbContext dbContext,
            IGatherLanguageMessageHandler langHandler, IBackgroundJobClient backgroundJobClient,
            ILogger<StartRegistrationMessageHandler> logger, IApplicationCultures cultures) : base(logger)
        {
            _dbContext = dbContext;
            _langHandler = langHandler;
            _backgroundJobClient = backgroundJobClient;
            _cultures = cultures;
        }

        protected override async Task Base(Messaging message)
        {
            var incompleteUser = new IncompleteUser(message.Sender.Id, _cultures.DefaultCulture.Culture.Name);

            await _dbContext.IncompleteUsers.AddAsync(incompleteUser);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            await _langHandler.ShowInstruction(incompleteUser);

            _backgroundJobClient.Schedule<EnsureRegistrationEndedJob>(
                x => x.EnsureRegistrationEnded(incompleteUser.Id),
                TimeSpan.FromMinutes(15)
                );
        }
    }
}