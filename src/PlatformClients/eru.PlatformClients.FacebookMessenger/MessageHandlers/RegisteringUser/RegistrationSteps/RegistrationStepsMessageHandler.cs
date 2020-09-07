using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser
{
    public abstract class RegistrationMessageHandler {}
    
    public abstract class RegistrationMessageHandler<T> : RegistrationMessageHandler where T : RegistrationMessageHandler
    {
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;

        protected RegistrationMessageHandler(ITranslator<FacebookMessengerPlatformClient> translator)
        {
            _translator = translator;
        }

        private async Task Gather(IncompleteUser user, string data)
        {
            //common Gather() code
            await GatherBase(user, data);
        }
        
        public async Task ShowInstruction(IncompleteUser user, int page)
        {
            await ShowInstructionBase(user, page);
        }

        private async Task UnsupportedCommand(IncompleteUser user)
        {
            await ShowUnsupportedCommandBase(user);
        }

        public async Task Handle(IncompleteUser user, Payload payload)
        {
            if (payload?.Id != null)
            {
                await Gather(user, payload.Id);
                return;
            }

            if (payload?.Page != null)
            {
                await ShowInstruction(user, payload.Page.Value);
                return;
            }

            await UnsupportedCommand(user);
        }

        protected abstract Task<IncompleteUser> GatherBase(IncompleteUser user, string data);
        protected abstract Task ShowInstructionBase(IncompleteUser user, int page);
        protected abstract Task ShowUnsupportedCommandBase(IncompleteUser user);

        protected async Task<IEnumerable<QuickReply>> GetSelector(Dictionary<string, string> items, int page, PayloadType payloadType, string displayCulture)
        {
            var offset = page * 10;

            var scope = items.Skip(offset).Take(10);

            var replies = scope.Select(x => new QuickReply(x.Key, x.Value)).ToList();

            if (page > 0)
            {
                replies.Add(new QuickReply(await _translator.TranslateString("previous-page", displayCulture), new Payload(payloadType, page - 1).ToJson()));
            }

            if (items.Count() - offset - 10 > 0)
            {
                replies.Add(new QuickReply(await _translator.TranslateString("next-page", displayCulture), new Payload(payloadType, page + 1).ToJson()));
            }

            replies.Add(new QuickReply(await _translator.TranslateString("cancel-button", displayCulture), new Payload(PayloadType.Cancel).ToJson()));

            return replies;
        } 
    }
}