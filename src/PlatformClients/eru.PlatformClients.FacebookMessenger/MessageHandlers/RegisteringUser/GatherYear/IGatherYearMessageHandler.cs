using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear
{
    public interface IGatherYearMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}