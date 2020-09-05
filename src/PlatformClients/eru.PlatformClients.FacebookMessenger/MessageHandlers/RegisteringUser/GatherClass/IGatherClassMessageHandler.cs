using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass
{
    public interface IGatherClassMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}