using eru.Application.Subscriptions.Commands.CreateSubscription;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Entities
{
    public class IncompleteUser
    {
        public string Id { get; set; } 
        public string PreferredLanguage { get; set; }
        public int Year { get; set; }
        public string ClassId { get; set; }
        public Stage Stage { get; set; }
        
        public int LastPage { get; set; }
        
        public CreateSubscriptionCommand ToCreateSubscriptionCommand() => new CreateSubscriptionCommand(Id, "FacebookMessenger", PreferredLanguage, ClassId);
        
    }
}