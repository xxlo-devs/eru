using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler
{
    public interface IRegisteringUserMessageHandler
    {
        public Task Handle(string uid, Message message);
    }
}