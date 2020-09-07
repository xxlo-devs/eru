using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps;
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
        private readonly RegistrationStepsMessageHandler<GatherLanguageMessageHandler> _langHandler;
        
        public StartRegistrationMessageHandler(IRegistrationDbContext dbContext, RegistrationStepsMessageHandler<GatherLanguageMessageHandler> langHandler , ILogger<StartRegistrationMessageHandler> logger) : base(logger)
        {
            _dbContext = dbContext;
            _langHandler = langHandler;
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