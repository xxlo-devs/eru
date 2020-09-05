using System.Threading;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext
{
    public interface IRegistrationDbContext
    {
        DbSet<IncompleteUser> IncompleteUsers { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}