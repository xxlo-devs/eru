using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear
{
    public interface IGatherYearMessageHandler
    {
        public Task Handle(string uid, Payload payload);
    }
}