using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand
{
    public interface IUnsupportedCommandMessageHandler
    {
        public Task Handle(string uid);
    }
}