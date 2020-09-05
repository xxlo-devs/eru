using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear
{
    public interface IGatherYearMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}