using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext
{
    public class RegistrationDbContext : Microsoft.EntityFrameworkCore.DbContext, IRegistrationDbContext
    {
        public RegistrationDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<IncompleteUser> IncompleteUsers { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IncompleteUser>(x =>
            {
                x.HasKey(y => y.Id);
                
                x.Property(y => y.Id)
                    .HasMaxLength(255);
                x.Property(y => y.ClassId)
                    .HasMaxLength(255);
                x.Property(y => y.PreferredLanguage)
                    .HasMaxLength(255);
            });
        }
    }
}