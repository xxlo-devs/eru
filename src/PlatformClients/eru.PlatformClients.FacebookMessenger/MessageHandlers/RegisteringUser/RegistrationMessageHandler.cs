using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public abstract class RegistrationMessageHandler {}
    
    public abstract class RegistrationMessageHandler<T> : RegistrationMessageHandler where T : RegistrationMessageHandler
    {
        public async Task Gather()
        {
            //common Gather() code
            await GatherBase();
        }
        
        public async Task ShowInstruction()
        {
            //common ShowInstruction() code
            await ShowInstructionBase();
        }

        public async Task UnsupportedCommand()
        {
            //common UnsupportedCommand() code
            await ShowUnsupportedCommandBase();
        }

        public async Task Handle(string uid, Payload payload)
        {
            if (payload?.Id != null)
            {
                await Gather();
                return;
            }

            if (payload?.Page != null)
            {
                await ShowInstruction();
                return;
            }

            await UnsupportedCommand();
        }
        
        public abstract Task GatherBase();
        public abstract Task ShowInstructionBase();
        public abstract Task ShowUnsupportedCommandBase();
    }
}