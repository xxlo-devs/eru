﻿using System.Diagnostics.CodeAnalysis;
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

        public DbSet<User> Users { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>(x =>
            {
                x.HasKey(y => y.Id);
                x.Property(y => y.Id)
                    .HasMaxLength(255);
                x.Property(y => y.Section)
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<User>(x =>
            {
                x.HasKey(y => new {y.Id, y.Platform});
                x.Property(y => y.Id).HasMaxLength(255);
                x.Property(y => y.Platform).HasMaxLength(255);
                x.Property(y => y.Class).HasMaxLength(255);
                x.Property(y => y.PreferredLanguage).HasMaxLength(255);
            });
        }
    }
}