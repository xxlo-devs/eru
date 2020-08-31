using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration
{
    public interface ICancelRegistrationMessageHandler
    {
        public Task Handle(string uid);
    }
}