using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage
{
    public interface IGatherLanguageMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}