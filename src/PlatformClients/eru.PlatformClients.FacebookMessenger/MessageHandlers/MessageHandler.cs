using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers
{
    public abstract class MessageHandler {}

    public abstract class MessageHandler<T> : MessageHandler where T : MessageHandler
    {
        private readonly ILogger<T> _logger;

        protected MessageHandler(ILogger<T> logger)
        {
            _logger = logger;
        }
        
        protected abstract Task Base(Messaging message);
        
        public virtual async Task Handle(Messaging message)
        {
            _logger.LogTrace($"tracelog");
            await Base(message);
            _logger.LogInformation($"infolog");
        }
    }
}