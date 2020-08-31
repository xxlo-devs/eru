using System;
using eru.Domain.Entity;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using Microsoft.EntityFrameworkCore;

namespace eru.Infrastructure.Tests.PlatformClients
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
                x.Property(y => y.Platform)
                    .HasMaxLength(255);
                x.Property(y => y.ClassId)
                    .HasMaxLength(255);
                x.Property(y => y.PreferredLanguage)
                    .HasMaxLength(255);
                
                x.HasData(
                    new IncompleteUser { Id = "sample-registering-user", Platform = "FacebookMessenger", ClassId = null, PreferredLanguage = null, Year = 0, Stage = Stage.Created},
                    new IncompleteUser { Id = "sample-registering-user-with-lang", Platform = "FacebookMessenger", ClassId = null, PreferredLanguage = "en-us", Year = 0, Stage = Stage.GatheredLanguage},
                    new IncompleteUser { Id = "sample-registering-user-with-year", Platform = "FacebookMessenger", ClassId = null, PreferredLanguage = "en-us", Year = 1, Stage = Stage.GatheredYear},
                    new IncompleteUser { Id = "sample-registering-user-with-class", Platform = "FacebookMessenger", ClassId = "a", PreferredLanguage = "en-us", Year = 1, Stage = Stage.GatheredClass}
                );
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}