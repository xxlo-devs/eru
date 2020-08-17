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
                x.HasKey(y => y.Name);
                x.Property(y => y.Name).HasMaxLength(255);

                x.HasData(new Class("I a"), new Class("II b"), new Class("III c"));
            });

            modelBuilder.Entity<User>(x =>
            {
                x.HasKey(y => new {y.Id, y.Platform});
                x.Property(y => y.Id).HasMaxLength(255);
                x.Property(y => y.Platform).HasMaxLength(255);
                x.Property(y => y.Class).HasMaxLength(255);
                x.Property(y => y.PreferredLanguage).HasMaxLength(255);

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