using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear
{
    public interface IGatherYearMessageHandler
    {
        public Task Handle(IncompleteUser user, Payload payload);
    }
}