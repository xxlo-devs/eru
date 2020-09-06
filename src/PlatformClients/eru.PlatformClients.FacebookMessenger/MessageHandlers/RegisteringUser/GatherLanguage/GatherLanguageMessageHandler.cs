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
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage
{
    public class GatherLanguageMessageHandler : IGatherLanguageMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly IConfiguration _configuration;
        private readonly ISelector _selector;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ILogger<GatherLanguageMessageHandler> _logger;

        public GatherLanguageMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, ISelector selector, IConfiguration configuration, ITranslator<FacebookMessengerPlatformClient> translator, ILogger<GatherLanguageMessageHandler> logger)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _selector = selector;
            _configuration = configuration;
            _translator = translator;
            _logger = logger;
        }
        public async Task Handle(string uid, Payload payload)
        {
            _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherLanguageMessageHandler.Handle got a request (uid: {uid}, payload: {payload.ToJson()})");
            
            if (payload.Type == PayloadType.Lang)
            {
                if (payload.Page != null)
                {
                    _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherLanguageMessageHandler.Handle redirected a request to GatherLanguageMessageHandler.ShowPage");
                    await ShowPage(uid, payload.Page.Value);
                    return;
                }

                if (payload.Id != null)
                {
                    _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherLanguageMessageHandler.Handle redirected a request to GatherLanguageMessageHandler.Gather");
                    await Gather(uid, payload.Id);
                    return;
                }
            }

            _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherLanguageMessageHandler.Handle redirected a request to GatherLanguageMessageHandler.UnsupportedCommand");
            await UnsupportedCommand(uid);
        }

        private async Task ShowPage(string uid, int page)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.LastPage = page; 
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("greeting", _configuration["CultureSettings:DefaultCulture"]), await _selector.GetLangSelector(user.LastPage)));
            await _apiClient.Send(response);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: GatherClassMessageHandler.ShowPage has successfully processed a request from user (uid: {uid}");
        }

        private async Task UnsupportedCommand(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            var response = new SendRequest(uid, 
                new Message(await _translator.TranslateString("unsupported-command", _configuration["CultureSettings:DefaultCulture"]), await _selector.GetLangSelector(user.LastPage)));
            await _apiClient.Send(response);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: GatherLanguageMessageHandler.UnsupportedCommand has successfully processed a request from user (uid: {uid}");
        }

        private async Task Gather(string uid, string lang)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.PreferredLanguage = lang; user.Stage = Stage.GatheredLanguage; user.LastPage = 0; 
            
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            var response = new SendRequest(uid, new Message(await _translator.TranslateString("year-selection", user.PreferredLanguage), await _selector.GetYearSelector(0, user.PreferredLanguage)));
            await _apiClient.Send(response);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: GatherLanguageMessageHandler.Gather has successfully appended language (iso code: {lang}) to user (uid: {uid}");
        }
    }
}