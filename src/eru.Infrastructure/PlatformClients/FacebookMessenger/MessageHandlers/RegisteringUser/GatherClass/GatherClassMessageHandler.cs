using System.Threading;
using System.Threading.Tasks;
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

        public GatherClassMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
        }
        
        public async Task Handle(string uid, string payload)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.ClassId = payload.Substring(ReplyPayloads.ClassPrefix.Length);
            user.Stage = Stage.GatheredClass;
            user.ListOffset = 0;
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var response = new SendRequest(uid, new Message("Now we have all the required informations to create your subscription. If you want to get a message about all substiututions concerning you as soon as the school publish that information, click the Subscribe button. If you want to delete (or modify) your data, click Cancel.", new []
            {
                new QuickReply("Subscribe", ReplyPayloads.SubscribePayload),
                new QuickReply("Cancel", ReplyPayloads.CancelPayload) 
            }));
            await _apiClient.Send(response);
        }
    }
}