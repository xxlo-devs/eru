using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription
{
    public interface IConfirmSubscriptionMessageHandler
    {
        public Task Handle(IncompleteUser user, Payload payload);
        public Task ShowInstruction(IncompleteUser user);
        public Task UnsupportedCommand(IncompleteUser user);
    }
}