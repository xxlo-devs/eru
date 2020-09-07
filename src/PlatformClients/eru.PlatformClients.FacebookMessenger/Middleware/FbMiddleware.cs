using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.Middleware
{
    public class FbMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly MessageHandler<IncomingMessageHandler> _messageHandler;
        private readonly ILogger<FbMiddleware> _logger;

        public FbMiddleware(IConfiguration configuration, MessageHandler<IncomingMessageHandler> messageHandler, ILogger<FbMiddleware> logger)
        {
            _configuration = configuration;
            _messageHandler = messageHandler;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            switch (context.Request.Method)
            {
                case "GET":
                    await VerifyWebhookRequest(context);
                    break;

                case "POST":
                    await HandleWebhookEvent(context);
                    break;

                default:
                    _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: FbMiddleware.InvokeAsync got a request with not supported method");
                    await context.SendResponse(HttpStatusCode.MethodNotAllowed);
                    break;
            }
        }

        private async Task VerifyWebhookRequest(HttpContext context)
        {
            string mode = context.Request.Query["hub.mode"];
            string token = context.Request.Query["hub.verify_token"];
            string challenge = context.Request.Query["hub.challenge"];

            if (mode != null && token != null && challenge != null)
            {
                if (mode == "subscribe" && token == _configuration["PlatformClients:FacebookMessenger:VerifyToken"])
                {
                    await context.SendOkResponse(challenge);
                    _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: FbMiddleware.VerifyWebhookRequest succesfully verified a webhook");
                }
                else
                {
                    await context.SendResponse(HttpStatusCode.Forbidden);
                    _logger.LogWarning($"eru.PlatformClients.FacebookMessenger: FbMiddleware.VerifyWebhookRequest got a verify requets with invalid token");
                }
            }
            else
            {
                await context.SendResponse(HttpStatusCode.BadRequest);
                _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: FbMiddleware.VerifyWebhookRequest got an invalid verify request");
            }
        }

        private async Task HandleWebhookEvent(HttpContext context)
        {
            try
            {
                var webhook = await JsonSerializer.DeserializeAsync<Event>(context.Request.Body);

                if (webhook.Subscription == "page")
                {
                    foreach (var x in webhook.Entry)
                    {
                        await _messageHandler.Handle(x.Messaging.First());
                    }
                    
                    await context.SendOkResponse("EVENT_RECEIVED");
                    _logger.LogInformation("eru.PlatformClients.FacebookMessenger: FbMiddleware.HandleWebhookEvent successfully processed an event");
                }
                else
                {
                    await context.SendResponse(HttpStatusCode.NotFound);
                    _logger.LogWarning($"eru.PlatformClients.FacebookMessenger: FbMiddleware.HandleWebhookEvent got an unsupported event");
                }
            }
            catch
            {
                await context.SendResponse(HttpStatusCode.BadRequest);
                _logger.LogError($"eru.PlatformClients.FacebookMessenger: FbMiddleware.HandleWebhookEvent couldn't process event");
            }
        }
    }
}