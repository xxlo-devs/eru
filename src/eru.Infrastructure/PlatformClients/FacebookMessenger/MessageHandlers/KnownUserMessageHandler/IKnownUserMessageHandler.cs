using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserMessageHandler
{
    public interface IKnownUserMessageHandler
    {
        public Task Handle(string uid, Message message);
    }
}