using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDbContext
{
    public interface IRegistrationDbContext
    {
        DbSet<IncompleteUser> IncompleteUsers { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}