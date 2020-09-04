using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription
{
    public interface IConfirmSubscriptionMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}