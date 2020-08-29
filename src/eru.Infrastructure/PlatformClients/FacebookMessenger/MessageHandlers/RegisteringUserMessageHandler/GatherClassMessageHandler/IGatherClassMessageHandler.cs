using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.GatherClassMessageHandler
{
    public interface IGatherClassMessageHandler
    {
        public Task Handle(string uid, string payload);
    }
}