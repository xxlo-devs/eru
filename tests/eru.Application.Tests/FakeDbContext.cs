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
            modelBuilder.Entity<Class>()
                .HasKey(x=>x.Name);
            modelBuilder.Entity<Class>()
                .Property(x => x.Name)
                .HasMaxLength(255);
            modelBuilder.Entity<Class>()
                .HasData(new Class("I a"), new Class("II b"), new Class("III c"));

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

            modelBuilder.Entity<User>()
                .HasData(
                    new User { Id = "sample-user", Platform = "DebugMessageService", Class = "II b", PreferredLanguage = "pl" }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}