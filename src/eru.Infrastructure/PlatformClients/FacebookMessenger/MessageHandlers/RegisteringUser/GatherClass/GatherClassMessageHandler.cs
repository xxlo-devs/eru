using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using Microsoft.EntityFrameworkCore.Storage;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass
{
    public class GatherClassMessageHandler : IGatherClassMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ISelector _selector;

        public GatherClassMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, ISelector selector)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _translator = translator;
            _selector = selector;
        }
        
        public async Task Handle(string uid, Payload payload)
        {
            if (payload.Type == PayloadType.Class)
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

        private async Task UnsupportedCommand(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), await _selector.GetClassSelector(user.LastPage, user.Year, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }

        private async Task ShowPage(string uid, int page)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.LastPage = page;
            
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
                            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("class-selection", user.PreferredLanguage), await _selector.GetClassSelector(user.LastPage, user.Year, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }

        private async Task Gather(string uid, string classId)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.ClassId = classId; user.LastPage = 0; user.Stage = Stage.GatheredClass; 
            
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("confirmation", user.PreferredLanguage), await _selector.GetConfirmationSelector(user.PreferredLanguage)));
            await _apiClient.Send(response);
        }
    }
}