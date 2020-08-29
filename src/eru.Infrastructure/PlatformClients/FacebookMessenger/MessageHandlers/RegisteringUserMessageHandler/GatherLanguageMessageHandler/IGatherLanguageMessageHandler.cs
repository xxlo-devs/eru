using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.GatherLanguageMessageHandler
{
    public interface IGatherLanguageMessageHandler
    {
        public Task Handle(string uid, string payload);
    }
}