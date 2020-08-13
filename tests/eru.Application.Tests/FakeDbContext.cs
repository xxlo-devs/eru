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
                .HasData(new Class("język Angielski"), new Class("język Polski"), new Class("matematyka"));

            modelBuilder.Entity<User>()
                .HasKey(k => new {k.Id, k.Platform});
            
            modelBuilder.Entity<User>()
                .HasData(
                    new User { Id = "98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F", Platform = Platform.DebugMessageService, Stage = Stage.Created, Year = Year.NotSupplied, Class = String.Empty },
                    new User { Id = "195CC4D0-80F5-4745-86AC-7FCD3BAF209B", Platform = Platform.DebugMessageService, Stage = Stage.GatheredYear, Year = Year.Freshman, Class = String.Empty},
                    new User { Id = "7124C49B-B04A-468F-A946-40025B19FF91", Platform = Platform.DebugMessageService, Stage = Stage.GatheredClass, Year = Year.Sophomore, Class = "język Polski"},
                    new User { Id = "380AE765-803D-4174-A370-1038B7D53CD6", Platform = Platform.DebugMessageService, Stage = Stage.Subscribed, Year = Year.Junior, Class = "III e"},
                    new User { Id = "FCDEE5DA-F755-45F9-B8BB-D7C7C303F70B", Platform = Platform.DebugMessageService, Stage = Stage.Cancelled, Year = Year.NotSupplied, Class = String.Empty}
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}