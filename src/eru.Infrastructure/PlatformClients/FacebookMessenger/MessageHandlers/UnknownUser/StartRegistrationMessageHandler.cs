using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using Microsoft.Extensions.Localization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
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