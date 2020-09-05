using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage
{
    public interface IGatherLanguageMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}