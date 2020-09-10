using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.CancelRegistration
{
    public interface ICancelRegistrationMessageHandler
    {
        public Task Handle(Messaging message);
    }
}