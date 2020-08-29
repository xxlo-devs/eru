using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.GatherYearMessageHandler
{
    public interface IGatherYearMessageHandler
    {
        public Task Handle(string uid, string payload);
    }
}