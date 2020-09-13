using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps
{
    public abstract class RegistrationStepsMessageHandler {}
    
    public abstract class RegistrationStepsMessageHandler<T> : RegistrationStepsMessageHandler
        where T : RegistrationStepsMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ILogger<T> _logger;
        
        protected RegistrationStepsMessageHandler(IRegistrationDbContext dbContext,
            ITranslator<FacebookMessengerPlatformClient> translator, ILogger<T> logger)
        {
            _dbContext = dbContext;
            _translator = translator;
            _logger = logger;
        }
        
        public async Task Handle(IncompleteUser user, Payload payload)
        {
            _logger.LogTrace(
                "Facebook Messenger Message Handler {typeof(T).Name} got a request from user {user.Id} with payload {payload}",
                typeof(T), user, payload);
            
            if (payload?.Id != null)
            {
                await UpdateUser(user, payload.Id);
                return;
            }

            if (payload?.Page != null)
            {
                await ShowInstruction(user, payload.Page.Value);
                return;
            }

            await UnsupportedCommand(user);
        }
        
        public async Task ShowInstruction(IncompleteUser user, int page = 0)
        {
            var usr = user;
            await ShowInstructionBase(usr, page);
            user.LastPage = page;

            _dbContext.IncompleteUsers.Update(usr);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            _logger.LogInformation(
                "Facebook Messenger Message Handler {typeof(T).Name} successfully sent page {page} for user {user.Id}",
                typeof(T), page, user);
        }

        private async Task UpdateUser(IncompleteUser user, string data)
        {
            var usr = await UpdateUserBase(user, data);
            usr.LastPage = 0;
            usr.Stage++;
            
            _dbContext.IncompleteUsers.Update(usr);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            _logger.LogInformation(
                "Facebook Messenger Message Handler {typeof(T).Name} successfully updated user {user.Id} with data {data}",
                typeof(T), user, data);
        }
        
        private async Task UnsupportedCommand(IncompleteUser user)
        {
            await UnsupportedCommandBase(user);
            _logger.LogInformation(
                "Facebook Messenger Message Handler {typeof(T).Name} successfully sent UnsupportedCommand response to user {user.Id}",
                typeof(T), user);
        }
        
        protected async Task<IEnumerable<QuickReply>> GetSelector(Dictionary<string, string> items, int page, PayloadType payloadType, string displayCulture)
        {
            var offset = page * 10;

            var scope = items
                .Skip(offset)
                .Take(10);

            var replies = scope
                .Select(x => new QuickReply(x.Key, x.Value))
                .ToList();

            if (page > 0)
            {
                replies.Add(new QuickReply(await _translator.TranslateString("previous-page", displayCulture),
                    new Payload(payloadType, page - 1).ToJson()));
            }

            if (items.Count - offset - 10 > 0)
            {
                replies.Add(new QuickReply(await _translator.TranslateString("next-page", displayCulture),
                    new Payload(payloadType, page + 1).ToJson()));
            }

            replies.Add(new QuickReply(await _translator.TranslateString("cancel-button", displayCulture),
                new Payload(PayloadType.Cancel).ToJson()));

            return replies;
        } 
        
        protected abstract Task<IncompleteUser> UpdateUserBase(IncompleteUser user, string data);
        protected abstract Task ShowInstructionBase(IncompleteUser user, int page);
        protected abstract Task UnsupportedCommandBase(IncompleteUser user);
    }
}