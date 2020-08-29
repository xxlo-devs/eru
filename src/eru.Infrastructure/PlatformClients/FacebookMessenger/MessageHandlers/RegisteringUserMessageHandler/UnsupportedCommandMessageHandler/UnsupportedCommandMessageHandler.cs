using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler.UnsupportedCommandMessageHandler
{
    public class UnsupportedCommandMessageHandler : IUnsupportedCommandMessageHandler
    {
        public Task Handle(string uid)
        {
            throw new System.NotImplementedException();
        }
    }
}