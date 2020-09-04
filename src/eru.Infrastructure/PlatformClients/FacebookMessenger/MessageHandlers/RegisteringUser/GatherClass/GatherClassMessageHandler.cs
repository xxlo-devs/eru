using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass
{
    public class GatherClassMessageHandler : IGatherClassMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;

        public GatherClassMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _translator = translator;
        }
        
        public async Task Handle(string uid, Payload payload)
        {
            if (payload.Type == Type.Class)
            {
                if (payload.Page != null)
                {
                    await ShowPage();
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
            throw new NotImplementedException();
        }

        private async Task ShowPage()
        {
            throw new NotImplementedException();
        }

        private async Task Gather(string uid, string classId)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.ClassId = classId;
            user.Stage = Stage.GatheredClass;
            user.ListOffset = 0;
            
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("confirmation", user.PreferredLanguage), new []
            {
                new QuickReply(await _translator.TranslateString("subscribe-button", user.PreferredLanguage), new Payload(Type.Subscribe).ToJson()),
                new QuickReply(await _translator.TranslateString("cancel-button", user.PreferredLanguage), new Payload(Type.Cancel).ToJson()) 
            }));
            await _apiClient.Send(response);
        }
    }
}