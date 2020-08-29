using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUserMessageHandler
{
    public class UnknownUserMessageHandler : IUnknownUserMessageHandler
    {
        public Task Handle(string uid)
        {
            throw new System.NotImplementedException();
        }
    }
}