using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass
{
    public interface IGatherClassMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}