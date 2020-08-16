using System;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Tests
{
    public sealed class FakeDbContext : DbContext, IApplicationDbContext
    {
        public FakeDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Class> Classes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseInMemoryDatabase($"eru.Application.Tests.{Guid.NewGuid().ToString()}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>(x =>
            {
                x.HasKey(x => x.Name);
                x.Property(x => x.Name).HasMaxLength(255);

                x.HasData(new Class("I a"), new Class("II b"), new Class("III c"));
            });

            modelBuilder.Entity<User>(x =>
            {
                x.HasKey(k => new {k.Id, k.Platform});
                x.Property(x => x.Id).HasMaxLength(255);
                x.Property(x => x.Platform).HasMaxLength(255);
                x.Property(x => x.Class).HasMaxLength(255);
                x.Property(x => x.PreferredLanguage).HasMaxLength(255);
                x.HasData(
                    new User { Id = "sample-user", Platform = "DebugMessageService", Class = "II b", PreferredLanguage = "pl" },
                    new User { Id = "sample-user-2", Platform = "DebugMessageService", Class = "I a", PreferredLanguage = "en"},
                    new User { Id = "sample-user-3", Platform = "DebugMessageService", Class = "II b", PreferredLanguage = "pl"}
                );
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}