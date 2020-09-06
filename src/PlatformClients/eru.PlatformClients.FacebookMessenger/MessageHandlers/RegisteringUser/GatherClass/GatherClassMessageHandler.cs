using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass
{
    public class GatherClassMessageHandler : IGatherClassMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ISelector _selector;
        private readonly ILogger<GatherClassMessageHandler> _logger;

        public GatherClassMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, ISelector selector, ILogger<GatherClassMessageHandler> logger)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _translator = translator;
            _selector = selector;
            _logger = logger;
        }
        
        public async Task Handle(string uid, Payload payload)
        {
            _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: GatherClassMessageHandler got a request (uid: {uid}, payload: {payload.ToJson()})");
            if (payload.Type == PayloadType.Class)
            {
                if (payload.Page != null)
                {
                    _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherClassMessageHandler.Handle redirected a request to GatherClassMessageHandler.ShowPage");
                    await ShowPage(uid, payload.Page.Value);
                    return; 
                }

                if (payload.Id != null)
                {
                    _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherClassMessageHandler.Handle redirected a request to GatherClassMessageHandler.Gather");
                    await Gather(uid, payload.Id);
                    return; 
                }
            }

            _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: GatherClassMessageHandler.Handle redirected a request to GatherClassMessageHandler.UnsupportedCommand");
            await UnsupportedCommand(uid);
        }

        private async Task UnsupportedCommand(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), await _selector.GetClassSelector(user.LastPage, user.Year, user.PreferredLanguage)));
            await _apiClient.Send(response);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: GatherClassMessageHandler.UnsupportedCommand has successfully processed a request from user (uid: {uid})");
        }

        private async Task ShowPage(string uid, int page)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.LastPage = page;
            
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
                            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("class-selection", user.PreferredLanguage), await _selector.GetClassSelector(user.LastPage, user.Year, user.PreferredLanguage)));
            await _apiClient.Send(response);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: GatherClassMessageHandler.ShowPage has successfully processed a request from user (uid: {uid}, page: {page})");
        }

        private async Task Gather(string uid, string classId)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.ClassId = classId; user.LastPage = 0; user.Stage = Stage.GatheredClass; 
            
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("confirmation", user.PreferredLanguage), await _selector.GetConfirmationSelector(user.PreferredLanguage)));
            await _apiClient.Send(response);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: GatherClassMessageHandler.Gather has successfully appended class (classId: {classId}) to user (uid: {uid}");
        }
    }
}