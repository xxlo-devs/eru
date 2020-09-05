using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass
{
    public interface IGatherClassMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}