namespace eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDbContext
{
    public class IncompleteUser
    {
        public string Id { get; set; } 
        public string Platform { get; set; }
        public string PreferredLanguage { get; set; }
        public int Year { get; set; }
        public string ClassId { get; set; }
        public Stage Stage { get; set; }
    }
}