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
            modelBuilder.Entity<Class>()
                .HasKey(x=>x.Name);
            modelBuilder.Entity<Class>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .HasKey(k => new {k.Id, k.Platform});
            modelBuilder.Entity<User>()
                .Property(x => x.Id)
                .HasMaxLength(255);
            modelBuilder.Entity<User>()
                .Property(x => x.Platform)
                .HasMaxLength(255);
            modelBuilder.Entity<User>()
                .Property(x => x.Class)
                .HasMaxLength(255);
            modelBuilder.Entity<User>()
                .Property(x => x.PreferredLanguage)
                .HasMaxLength(255);
        }
    }
}