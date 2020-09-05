using eru.Application.Subscriptions.Commands.CreateSubscription;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;

namespace eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities
{
    public class IncompleteUser
    {
        public IncompleteUser(string id)
        {
            Id = id;
            Stage = Stage.Created;
        }
        
        public CreateSubscriptionCommand ToCreateSubscriptionCommand() => new CreateSubscriptionCommand(Id, FacebookMessengerPlatformClient.PId, PreferredLanguage, ClassId);

        public string Id { get; set; } 
        public string PreferredLanguage { get; set; }
        public int Year { get; set; }
        public string ClassId { get; set; }
        public int LastPage { get; set; }
        public Stage Stage { get; set; }
    }
}