using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public interface IStartRegistrationMessageHandler
    {
        public Task Handle(Messaging message);
    }
}