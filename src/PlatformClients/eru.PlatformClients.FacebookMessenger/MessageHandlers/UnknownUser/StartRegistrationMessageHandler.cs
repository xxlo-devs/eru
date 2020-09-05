using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Hangfire.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public class StartRegistrationMessageHandler : IUnkownUserMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _client;
        private readonly IConfiguration _configuration;
        private readonly ISelector _selector;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ILogger _logger;
            
        public StartRegistrationMessageHandler(IRegistrationDbContext dbContext, ISendApiClient client, IConfiguration configuration, ISelector selector, ITranslator<FacebookMessengerPlatformClient> translator, ILogger logger)
        {
            _dbContext = dbContext;
            _client = client;
            _configuration = configuration;
            _selector = selector;
            _translator = translator;
            _logger = logger;
        }
        
        public async Task Handle(string uid)
        {
            _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: StartRegistrationMessageHandler.Handle got a message (uid: {uid})");
            var incompleteUser = new IncompleteUser(uid);

            await _dbContext.IncompleteUsers.AddAsync(incompleteUser);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("greeting", _configuration["CultureSettings:DefaultCulture"]), await _selector.GetLangSelector(0)));
            await _client.Send(response);
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: StartRegistrationMessageHandler.Handle successfully started registration of new user (uid: {uid})");
        }
    }
}