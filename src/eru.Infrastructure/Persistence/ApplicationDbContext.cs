using System.Diagnostics.CodeAnalysis;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace eru.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<Class> Classes { get; set; }

        public DbSet<User> Users { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>(x =>
            {
                x.HasKey(x => x.Name);
                x.Property(x => x.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<User>(x =>
            {
                x.HasKey(k => new {k.Id, k.Platform});
                x.Property(x => x.Id).HasMaxLength(255);
                x.Property(x => x.Platform).HasMaxLength(255);
                x.Property(x => x.Class).HasMaxLength(255);
                x.Property(x => x.PreferredLanguage).HasMaxLength(255);
            });
        }
    }
}