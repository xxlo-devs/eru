using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUserMessageHandler
{
    public interface IUnknownUserMessageHandler
    {
        public Task Handle(string uid);
    }
}