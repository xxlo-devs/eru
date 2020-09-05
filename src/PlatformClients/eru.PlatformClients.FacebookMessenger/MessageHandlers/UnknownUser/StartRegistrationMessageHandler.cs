using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Microsoft.Extensions.Configuration;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public class StartRegistrationMessageHandler : IUnkownUserMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _client;
        private readonly IConfiguration _configuration;
        private readonly ISelector _selector;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
            
        public StartRegistrationMessageHandler(IRegistrationDbContext dbContext, ISendApiClient client, IConfiguration configuration, ISelector selector, ITranslator<FacebookMessengerPlatformClient> translator)
        {
            _dbContext = dbContext;
            _client = client;
            _configuration = configuration;
            _selector = selector;
            _translator = translator;
        }
        
        public async Task Handle(string uid)
        {
            var incompleteUser = new IncompleteUser(uid);

            await _dbContext.IncompleteUsers.AddAsync(incompleteUser);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("greeting", _configuration["CultureSettings:DefaultCulture"]), await _selector.GetLangSelector(0)));
            await _client.Send(response);
        }
    }
}