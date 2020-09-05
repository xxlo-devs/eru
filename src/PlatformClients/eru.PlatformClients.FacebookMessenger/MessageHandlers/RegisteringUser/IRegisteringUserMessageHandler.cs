using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public interface IRegisteringUserMessageHandler
    {
        public Task Handle(string uid, Message message);
    }
}