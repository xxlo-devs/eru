using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
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
        
        public async Task Handle(Messaging message)
        {
            _logger.LogTrace("Facebook Messenger Message Handler {typeof(T).Name} got a request (message: {message})",
                typeof(T).Name, message);
            await Base(message);
            _logger.LogInformation(
                "Facebook Messenger Message Handler {typeof(T).Name} successfully handled request from user {message.Sender.Id}",
                typeof(T).Name, message.Sender.Id);
        }
    }
}