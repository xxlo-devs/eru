using eru.Application.Users.Commands.CreateUser;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDbContext
{
    public class IncompleteUser
    {
        public string Id { get; set; }
        public string Platform { get; set; }
        public string Class { get; set; }
        public string PreferredLanguage { get; set; }

        public Stage Stage { get; set; }
        public int ClassOffset { get; set; }
    }
}