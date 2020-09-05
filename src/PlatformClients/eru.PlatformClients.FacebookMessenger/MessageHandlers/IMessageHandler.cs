using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers
{
    public interface IMessageHandler
    {
        public Task Handle(Messaging message);
    }
}