﻿using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IConfiguration _configuration;
        private readonly IGatherLanguageMessageHandler _langHandler;
        private readonly IBackgroundJobClient _backgroundJobClient;
        
        public StartRegistrationMessageHandler(IRegistrationDbContext dbContext, IConfiguration configuration,
            IGatherLanguageMessageHandler langHandler, IBackgroundJobClient backgroundJobClient,
            ILogger<StartRegistrationMessageHandler> logger) : base(logger)
        {
            _dbContext = dbContext;
            _langHandler = langHandler;
            _configuration = configuration;
            _backgroundJobClient = backgroundJobClient;
        }

        protected override async Task Base(Messaging message)
        {
            var incompleteUser = new IncompleteUser(message.Sender.Id, _configuration["CultureSettings:DefaultCulture"]);

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