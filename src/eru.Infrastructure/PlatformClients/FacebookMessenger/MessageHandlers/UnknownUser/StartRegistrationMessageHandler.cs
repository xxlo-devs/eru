using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            
        public StartRegistrationMessageHandler(IRegistrationDbContext dbContext, ISendApiClient client, IConfiguration configuration, ISelector selector)
        {
            _dbContext = dbContext;
            _client = client;
            _configuration = configuration;
            _selector = selector;
        }
        
        public async Task Handle(string uid)
        {
            var incompleteUser = new IncompleteUser
            {
                Id = uid,
                Platform = "FacebookMessenger",
                Stage = Stage.Created
            };

            await _dbContext.IncompleteUsers.AddAsync(incompleteUser);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var supportedCultures = _configuration.GetSection("CultureSettings:AvailableCultures").AsEnumerable().Select(x => x.Value).Skip(1);
            var dict = new Dictionary<string, string>();
            foreach (var x in supportedCultures)
            {
                var culture = new CultureInfo(x);
                dict.Add(culture.DisplayName, $"{ReplyPayloads.LangPrefix}{x}");
            }
            
            var response = new SendRequest(uid, new Message("Hello! Eru is a substitution information system that enables you to get personalized notifications about all substitutions directly from school. If you want to try it, choose your language by clicking on a correct flag below. If you don't want to use this bot, just click Cancel at any time.", _selector.GetSelector(dict, incompleteUser.ListOffset)));
            await _client.Send(response);
        }
    }
}