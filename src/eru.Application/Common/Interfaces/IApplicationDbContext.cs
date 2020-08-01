using eru.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Class> Classes { get; set; }
    }
}