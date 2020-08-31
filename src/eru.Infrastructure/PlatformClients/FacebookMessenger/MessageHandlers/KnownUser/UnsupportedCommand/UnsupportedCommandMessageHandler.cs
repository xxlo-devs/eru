using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand
{
    public class UnsupportedCommandMessageHandler : IUnsupportedCommandMessageHandler
    {
        public async Task Handle(string uid)
        {
            throw new System.NotImplementedException();
        }
    }
}