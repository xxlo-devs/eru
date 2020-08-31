using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear
{
    public interface IGathterYearMessageHandler
    {
        public Task Handle(string uid, string payload);
    }
}