using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUserMessageHandler.CreateUserMessageHandler
{
    public interface ICreateUserMessageHandler
    {
        public Task Handle(string uid);
    }
}