using System.Threading;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext
{
    public interface IRegistrationDbContext
    {
        DbSet<IncompleteUser> IncompleteUsers { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}