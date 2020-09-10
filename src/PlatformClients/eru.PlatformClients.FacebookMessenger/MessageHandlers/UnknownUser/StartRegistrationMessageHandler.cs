using System.Threading;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public class StartRegistrationMessageHandler : MessageHandler<StartRegistrationMessageHandler>, IStartRegistrationMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly RegistrationStepsMessageHandler<GatherLanguageMessageHandler> _langHandler;
        
        public StartRegistrationMessageHandler(IRegistrationDbContext dbContext, IConfiguration configuration, RegistrationStepsMessageHandler<GatherLanguageMessageHandler> langHandler , ILogger<StartRegistrationMessageHandler> logger) : base(logger)
        {
            _dbContext = dbContext;
            _langHandler = langHandler;
            _configuration = configuration;
        }

        protected override async Task Base(Messaging message)
        {
            var incompleteUser = new IncompleteUser(message.Sender.Id, _configuration["CultureSettings:DefaultCulture"]);

            await _dbContext.IncompleteUsers.AddAsync(incompleteUser);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            await _langHandler.ShowInstruction(incompleteUser);
        }
    }
}