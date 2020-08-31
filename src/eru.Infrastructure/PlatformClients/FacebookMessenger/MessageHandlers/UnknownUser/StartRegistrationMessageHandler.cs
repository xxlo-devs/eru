using System.Threading;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using Microsoft.Extensions.Localization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public class StartRegistrationMessageHandler : IUnkownUserMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _client; 
        public StartRegistrationMessageHandler(IRegistrationDbContext dbContext, ISendApiClient client)
        {
            _dbContext = dbContext;
            _client = client;
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
            
            var response = new SendRequest(uid, new Message("Hello! Eru is a substitution information system that enables you to get personalized notifications about all substitutions directly from school. If you want to try it, choose your language by clicking on a correct flag below. If you don't want to use this bot, just click Cancel at any time.", new []
            {
                new QuickReply("English", "en"),
                new QuickReply("Cancel", ReplyPayloads.CancelPayload), 
            }));

            await _client.Send(response);
        }
    }
}