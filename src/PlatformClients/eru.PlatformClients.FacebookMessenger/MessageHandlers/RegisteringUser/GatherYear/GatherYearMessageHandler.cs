using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Hangfire.Logging;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear
{
    public class GatherYearMessageHandler : IGatherYearMessageHandler
    {        
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly ISelector _selector;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ILogger _logger;
        
        public GatherYearMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, ISelector selector, ITranslator<FacebookMessengerPlatformClient> translator, ILogger logger)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _selector = selector;
            _translator = translator;
            _logger = logger;
        }
        public async Task Handle(string uid, Payload payload)
        {
            _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherYearMessageHandler.Handle got a request (uid: {uid}, payload: {payload.ToJson()})");
            
            if (payload.Type == PayloadType.Year)
            {
                if (payload.Page != null)
                {
                    _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherYearMessageHandler.Handle redirected a request to GatherYearMessageHandler.ShowPage");
                    await ShowPage(uid, payload.Page.Value);
                    return;
                }

                if (payload.Id != null)
                {
                    _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherYearMessageHandler.Handle redirected a request to GatherYearMessageHandler.Gather");
                    await Gather(uid, int.Parse(payload.Id));
                    return; 
                }
            }

            _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherYearMessageHandler.Handle redirected a request to GatherYearMessageHandler.UnsupportedCommand");
            await UnsupportedCommand(uid);
        }

        private async Task ShowPage(string uid, int page)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.LastPage = page;

            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("year-selection", user.PreferredLanguage), await _selector.GetYearSelector(user.LastPage, user.PreferredLanguage)));
            await _apiClient.Send(response);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: GatherYearMessageHandler.ShowPage has successfully processed a request from user (uid: {uid}, page: {page}");
        }

        private async Task UnsupportedCommand(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);

            var response = new SendRequest(uid, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), await _selector.GetYearSelector(user.LastPage, user.PreferredLanguage)));
            await _apiClient.Send(response);   
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: GatherYearMessageHandler.UnsupportedCommand has successfully processed a request from user (uid: {uid}");
        }

        private async Task Gather(string uid, int year)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.Year = year; user.LastPage = 0; user.Stage = Stage.GatheredYear; 
            
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            var response = new SendRequest(uid, new Message(await _translator.TranslateString("class-selection", user.PreferredLanguage), await _selector.GetClassSelector(0, user.Year, user.PreferredLanguage)));
            await _apiClient.Send(response);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: GatherYearMessageHandler.UnsupportedCommand has successfully appended year (year: {year}) to user (uid: {uid}");
        }
    }
}