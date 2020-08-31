using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public interface IUnkownUserMessageHandler
    {
        public Task Handle(string uid);
    }
}