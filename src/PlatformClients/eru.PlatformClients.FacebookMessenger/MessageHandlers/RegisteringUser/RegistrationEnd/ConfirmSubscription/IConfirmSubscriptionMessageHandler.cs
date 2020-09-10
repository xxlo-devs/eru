using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription
{
    public interface IConfirmSubscriptionMessageHandler
    {
        public Task Handle(Messaging message);
        public Task ShowInstruction(IncompleteUser user);
    }
}