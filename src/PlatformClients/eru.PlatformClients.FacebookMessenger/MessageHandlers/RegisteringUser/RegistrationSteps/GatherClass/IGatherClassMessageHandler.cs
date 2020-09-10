using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass
{
    public interface IGatherClassMessageHandler
    {
        public Task Handle(IncompleteUser user, Payload payload);
        public Task ShowInstruction(IncompleteUser user, int page = 0);
    }
}