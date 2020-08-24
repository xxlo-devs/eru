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
                x.HasKey(y => y.Id);
                x.Property(y => y.Id)
                    .HasMaxLength(255);
                x.Property(y => y.Section)
                    .HasMaxLength(255);

                x.HasData(
                    new Class(1, "a") {Id = "sample-class"},
                    new Class(2, "b") {Id = "sample-class-2"},
                    new Class(3, "c") {Id = "sample-class-3"}
                );
            });

            modelBuilder.Entity<User>(x =>
            {
                x.HasKey(y => new {y.Id, y.Platform});
                x.Property(y => y.Id).HasMaxLength(255);
                x.Property(y => y.Platform).HasMaxLength(255);
                x.Property(y => y.Class).HasMaxLength(255);
                x.Property(y => y.PreferredLanguage).HasMaxLength(255);

                x.HasData(
                    new User { Id = "sample-user", Platform = "DebugMessageService", Class = "sample-class-2", PreferredLanguage = "pl" },
                    new User { Id = "sample-user-2", Platform = "DebugMessageService", Class = "sample-class", PreferredLanguage = "en"},
                    new User { Id = "sample-user-3", Platform = "DebugMessageService", Class = "sample-class-2", PreferredLanguage = "pl"}
                );
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}