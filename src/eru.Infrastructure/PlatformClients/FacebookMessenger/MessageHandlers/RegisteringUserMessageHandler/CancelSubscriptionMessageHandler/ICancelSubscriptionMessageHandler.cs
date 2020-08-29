using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.CancelSubscriptionMessageHandler
{
    public interface ICancelSubscriptionMessageHandler
    {
        public Task Handle(string uid);
    }
}