using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration
{
    public class CancelRegistrationMessageHandler : ICancelRegistrationMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;

        public CancelRegistrationMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
        }
        public async Task Handle(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            _dbContext.IncompleteUsers.Remove(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            var response = new SendRequest(uid, new Message("We are sorry to see you go. Your subscription (and your data) has been deleted. If you will ever want to subscribe again, write anything to start the process."));
            await _apiClient.Send(response);
        }
    }
}