using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserMessageHandler.CancelSubscriptionHandler
{
    public interface ICancelSubscriptionMessageHandler
    {
        public Task Handle(string uid);
    }
}