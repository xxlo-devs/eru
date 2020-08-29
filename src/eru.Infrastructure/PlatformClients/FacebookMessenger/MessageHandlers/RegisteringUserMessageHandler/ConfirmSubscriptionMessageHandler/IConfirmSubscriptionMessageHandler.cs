using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.ConfirmSubscriptionMessageHandler
{
    public interface IConfirmSubscriptionMessageHandler
    {
        public Task Handle(string uid, string payload);
    }
}