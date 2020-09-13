using System.Threading;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using Microsoft.EntityFrameworkCore;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    // Class is instantiated  by Hangfire
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EnsureRegistrationEndedJob
    {
        private readonly IRegistrationDbContext _dbContext;

        public EnsureRegistrationEndedJob()
        {
            _dbContext = new RegistrationDbContext(new DbContextOptions<RegistrationDbContext>());
        }
        
        public async Task EnsureRegistrationEnded(string uid)
        {            
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            if (user != null)
            {
                _dbContext.IncompleteUsers.Remove(user);
                await _dbContext.SaveChangesAsync(CancellationToken.None);
            }
        }
    }
}