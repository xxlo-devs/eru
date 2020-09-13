using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<SubstitutionsRecord> SubstitutionsRecords { get; set; }

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
                    .HasMaxLength(255)
                    .ValueGeneratedOnAdd();
                x.Property(y => y.Section)
                    .HasMaxLength(255);

                x.HasData(
                    new Class(1, "a") {Id = "sample-class-id"}
                );
            });

            modelBuilder.Entity<Subscriber>(x =>
            {
                x.HasKey(y => new {y.Id, y.Platform});
                x.Property(y => y.Id).HasMaxLength(255);
                x.Property(y => y.Platform).HasMaxLength(255);
                x.Property(y => y.Class).HasMaxLength(255);
                x.Property(y => y.PreferredLanguage).HasMaxLength(255);

                x.HasData(
                    new Subscriber { Id = "sample-subscriber", Platform = "DebugMessageService", Class = "sample-class-id", PreferredLanguage = "pl" }
                );
            });

            modelBuilder.Entity<SubstitutionsRecord>(x =>
            {
                x.HasKey(y => y.UploadDateTime);
                x.Property(y => y.Substitutions)
                    .HasConversion(
                        substitutions => JsonSerializer.Serialize(substitutions, null),
                        json => JsonSerializer.Deserialize<IEnumerable<Substitution>>(json, null));
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}