using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public interface IRegisteringUserMessageHandler
    {
        public Task Handle(Messaging message);
    }
}