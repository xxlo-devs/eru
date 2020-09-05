using System.Threading.Tasks;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.CancelSubscription
{
    public interface ICancelSubscriptionMessageHandler
    {
        public Task Handle(string uid);
    }
}