using Microsoft.EntityFrameworkCore;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDbContext
{
    public class FacebookMessengerRegistrationDbContext : DbContext
    {
        public FacebookMessengerRegistrationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<IncompleteUser> IncompleteUsers { get; set; }
    }
}