using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers
{
    public interface IMessageHandler
    {
        public Task Handle(Messaging message);
    }
}