using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription
{
    public interface IConfirmSubscriptionMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}