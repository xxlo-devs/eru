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
                x.Property(y => y.ClassId)
                    .HasMaxLength(255);
                x.Property(y => y.PreferredLanguage)
                    .HasMaxLength(255);
                
                x.HasData(
                    new IncompleteUser { Id = "sample-registering-user", ClassId = null, PreferredLanguage = null, Year = 0, Stage = Stage.Created, ListOffset = 0},
                    new IncompleteUser { Id = "sample-registering-user-with-lang", ClassId = null, PreferredLanguage = "en", Year = 0, Stage = Stage.GatheredLanguage, ListOffset = 0},
                    new IncompleteUser { Id = "sample-registering-user-with-year", ClassId = null, PreferredLanguage = "en", Year = 1, Stage = Stage.GatheredYear, ListOffset = 0},
                    new IncompleteUser { Id = "sample-registering-user-with-class", ClassId = "sample-class", PreferredLanguage = "en", Year = 1, Stage = Stage.GatheredClass, ListOffset = 0},
                    new IncompleteUser { Id = "language-paging-test-user", ClassId = null, PreferredLanguage = null, Year = 0, Stage = Stage.Created, ListOffset = 10},
                    new IncompleteUser { Id = "year-paging-test-user", ClassId = null, PreferredLanguage = "en", Year = 0, Stage = Stage.GatheredLanguage, ListOffset = 10},
                    new IncompleteUser { Id = "class-paging-test-user", ClassId = null, PreferredLanguage = "en", Year = 1, Stage = Stage.GatheredYear, ListOffset = 10}
                );
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}