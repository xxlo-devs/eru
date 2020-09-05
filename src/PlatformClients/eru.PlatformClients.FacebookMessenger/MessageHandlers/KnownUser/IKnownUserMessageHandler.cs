using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser
{
    public interface IKnownUserMessageHandler
    {
        public Task Handle(string uid, Message message);
    }
}