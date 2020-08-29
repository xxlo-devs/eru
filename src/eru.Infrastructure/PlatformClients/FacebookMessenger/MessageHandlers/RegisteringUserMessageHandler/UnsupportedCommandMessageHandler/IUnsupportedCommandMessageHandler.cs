using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.UnsupportedCommandMessageHandler
{
    public interface IUnsupportedCommandMessageHandler
    {
        public Task Handle(string uid);
    }
}