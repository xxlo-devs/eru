using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserMessageHandler.UnknownCommandMessageHandler
{
    public interface IUnsupportedCommandMessageHandler
    {
        public Task Handle(string uid);
    }
}