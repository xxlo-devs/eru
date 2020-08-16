using System;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using eru.Domain.Enums;
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
                .HasData(
                    new User { Id = "98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F", Platform = "DebugMessageService", Stage = Stage.Created, Class = String.Empty, PreferredLanguage = "pl"},
                    new User { Id = "01906EAE-3E0B-4905-9ADE-4FA2104C6459", Platform = "DebugMessageService", Stage = Stage.GatheredLanguage, Class = String.Empty, PreferredLanguage = "pl"},
                    new User { Id = "7124C49B-B04A-468F-A946-40025B19FF91", Platform = "DebugMessageService", Stage = Stage.GatheredClass, Class = "II b", PreferredLanguage = "pl"},
                    new User { Id = "380AE765-803D-4174-A370-1038B7D53CD6", Platform = "DebugMessageService", Stage = Stage.Subscribed, Class = "III c", PreferredLanguage = "pl"},
                    new User { Id = "FCDEE5DA-F755-45F9-B8BB-D7C7C303F70B", Platform = "DebugMessageService", Stage = Stage.Cancelled, Class = String.Empty, PreferredLanguage = "pl"}
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}