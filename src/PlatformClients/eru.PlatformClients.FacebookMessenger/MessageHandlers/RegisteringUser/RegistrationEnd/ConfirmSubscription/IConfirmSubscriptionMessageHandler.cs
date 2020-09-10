using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription
{
    public interface IConfirmSubscriptionMessageHandler
    {
        public Task Handle(Messaging message);
    }
}