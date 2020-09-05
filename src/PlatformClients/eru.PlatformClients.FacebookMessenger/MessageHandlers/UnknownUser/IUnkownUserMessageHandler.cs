using System.Threading.Tasks;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser
{
    public interface IUnkownUserMessageHandler
    {
        public Task Handle(string uid);
    }
}