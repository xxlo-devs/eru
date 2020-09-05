using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Microsoft.Extensions.Configuration;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage
{
    public class GatherLanguageMessageHandler : IGatherLanguageMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly IConfiguration _configuration;
        private readonly ISelector _selector;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;

        public GatherLanguageMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, ISelector selector, IConfiguration configuration, ITranslator<FacebookMessengerPlatformClient> translator)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _selector = selector;
            _configuration = configuration;
            _translator = translator;
        }
        public async Task Handle(string uid, Payload payload)
        {
            if (payload.Type == PayloadType.Lang)
            {
                if (payload.Page != null)
                {
                    await ShowPage(uid, payload.Page.Value);
                    return;
                }

                if (payload.Id != null)
                {
                    await Gather(uid, payload.Id);
                    return;
                }
            }

            await UnsupportedCommand(uid);
        }

        private async Task ShowPage(string uid, int page)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.LastPage = page; 
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("greeting", _configuration["CultureSettings:DefaultCulture"]), await _selector.GetLangSelector(user.LastPage)));
            await _apiClient.Send(response);
        }

        private async Task UnsupportedCommand(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            var response = new SendRequest(uid, 
                new Message(await _translator.TranslateString("unsupported-command", _configuration["CultureSettings:DefaultCulture"]), await _selector.GetLangSelector(user.LastPage)));
            await _apiClient.Send(response);
        }

        private async Task Gather(string uid, string lang)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.PreferredLanguage = lang; user.Stage = Stage.GatheredLanguage; user.LastPage = 0; 
            
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            var response = new SendRequest(uid, new Message(await _translator.TranslateString("year-selection", user.PreferredLanguage), await _selector.GetYearSelector(0, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }
    }
}