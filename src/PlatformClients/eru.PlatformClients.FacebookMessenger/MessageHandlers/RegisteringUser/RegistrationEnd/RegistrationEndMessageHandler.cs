using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using Microsoft.Extensions.Logging;
using Message = eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Message;

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
            _logger.LogTrace($"EndRegistrationHandler {typeof(T).Name} got a request from user (id: {user.Id})");
            if (payload.Type == PayloadType.Subscribe)
            {
                await EndRegistration(user);
                return; 
            }

            await UnsupportedCommand(user);
        }
        
        protected abstract Task EndRegistration(IncompleteUser user);
        public abstract Task ShowInstruction(IncompleteUser user);
        protected abstract Task UnsupportedCommand(IncompleteUser user);
    }
}