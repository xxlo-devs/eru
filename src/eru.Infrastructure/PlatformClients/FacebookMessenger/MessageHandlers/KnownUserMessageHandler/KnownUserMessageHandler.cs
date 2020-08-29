using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserMessageHandler
{
    public class KnownUserMessageHandler : IKnownUserMessageHandler
    {
        public Task Handle(string uid, Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}