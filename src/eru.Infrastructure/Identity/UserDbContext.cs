using eru.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace eru.Infrastructure.Identity
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("users");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(x=>
            {
                x.Property(y => y.Id)
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(255);
                x.Property(y => y.Username)
                    .HasMaxLength(255);

                x.HasKey(y => y.Id);
            });
        }
    }
}