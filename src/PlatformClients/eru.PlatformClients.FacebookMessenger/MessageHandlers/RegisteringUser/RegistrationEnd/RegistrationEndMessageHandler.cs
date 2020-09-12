using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd
{
    public class RegistrationEndMessageHandler { }
    
    public abstract class RegistrationEndMessageHandler<T> : RegistrationEndMessageHandler where T : RegistrationEndMessageHandler
    {
        private readonly ILogger<T> _logger;

        protected RegistrationEndMessageHandler(ILogger<T> logger)
        {
            _logger = logger;
        }
        
        public async Task Handle(IncompleteUser user, Payload payload)
        {
            _logger.LogTrace($"Facebook Messenger Message Handler {typeof(T).Name} got a request from user (id: {user.Id})");
            if (payload.Type == PayloadType.Subscribe)
            {
                await EndRegistration(user);
                _logger.LogInformation($"Facebook Messenger Message Handler {typeof(T).Name} successfully registered user {user.Id}");
                return; 
            }

            await UnsupportedCommand(user);
            _logger.LogInformation($"Facebook Messenger Message Handler {typeof(T).Name} successfully sent UnsupportedCommand response to user {user.Id}");
        }
        
        protected abstract Task EndRegistration(IncompleteUser user);
        public abstract Task ShowInstruction(IncompleteUser user);
        protected abstract Task UnsupportedCommand(IncompleteUser user);
    }
}