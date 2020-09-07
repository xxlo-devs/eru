using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using Message = eru.PlatformClients.FacebookMessenger.Models.SendApi.Message;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd
{
    public class RegistrationEndMessageHandler { }
    
    public abstract class RegistrationEndMessageHandler<T> : RegistrationEndMessageHandler where T : RegistrationEndMessageHandler
    {
        public async Task Handle(Messaging message)
        {
            await EndRegistration(message);
        }
        
        protected abstract Task EndRegistration(Messaging message);
        public abstract Task ShowInstruction(IncompleteUser user);
    }
}