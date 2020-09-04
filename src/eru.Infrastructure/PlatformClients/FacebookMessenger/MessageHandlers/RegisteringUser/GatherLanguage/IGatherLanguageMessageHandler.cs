using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage
{
    public interface IGatherLanguageMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}