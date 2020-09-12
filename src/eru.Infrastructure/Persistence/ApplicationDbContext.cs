using System.Diagnostics.CodeAnalysis;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace eru.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<Class> Classes { get; set; }

        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<SubstitutionsRecord> SubstitutionsRecords { get; set; }

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
            });

            modelBuilder.Entity<Subscriber>(x =>
            {
                x.HasKey(y => new {y.Id, y.Platform});
                x.Property(y => y.Id).HasMaxLength(255);
                x.Property(y => y.Platform).HasMaxLength(255);
                x.Property(y => y.Class).HasMaxLength(255);
                x.Property(y => y.PreferredLanguage).HasMaxLength(255);
            });

            modelBuilder.Entity<Substitution>(x =>
            {
                x.HasKey(y => y.Id);
                x.Property(y => y.Groups)
                    .HasMaxLength(255);
                x.Property(y => y.Id)
                    .HasMaxLength(255)
                    .ValueGeneratedOnAdd();
                x.Property(y => y.Note)
                    .HasMaxLength(255);
                x.Property(y => y.Room)
                    .HasMaxLength(255);
                x.Property(y => y.Subject)
                    .HasMaxLength(255);
                x.Property(y => y.Substituting)
                    .HasMaxLength(255);
                x.Property(y => y.Teacher)
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<SubstitutionsRecord>(x =>
            {
                x.HasKey(y => y.UploadDateTime);
            });
        }
    }
}