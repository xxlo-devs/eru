using System.Threading;
using System.Threading.Tasks;
using eru.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Class> Classes { get; set; }
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}