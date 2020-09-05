using System;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using Microsoft.EntityFrameworkCore;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers
{
    public class FakeRegistrationDb : DbContext, IRegistrationDbContext
    {
        public FakeRegistrationDb()
        {
            Database.EnsureCreated();
        }
        public DbSet<IncompleteUser> IncompleteUsers { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseInMemoryDatabase($"eru.Application.Tests.{Guid.NewGuid().ToString()}");
        }

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
                
                x.HasData(
                    new IncompleteUser("sample-registering-user"),
                    new IncompleteUser("sample-registering-user-with-lang") {PreferredLanguage = "en", Stage = Stage.GatheredLanguage},
                    new IncompleteUser("sample-registering-user-with-year") {PreferredLanguage = "en", Year = 1, Stage = Stage.GatheredYear},
                    new IncompleteUser("sample-registering-user-with-class") {PreferredLanguage = "en", Year = 1, ClassId = "sample-class", Stage = Stage.GatheredClass}
                );
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}