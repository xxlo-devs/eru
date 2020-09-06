using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.Models.Webhook;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger
{
    public class FbMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger<FbMiddleware> _logger;

        public FbMiddleware(IConfiguration configuration, IMessageHandler messageHandler, ILogger<FbMiddleware> logger)
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
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    byte[] response = Encoding.UTF8.GetBytes(string.Empty);
                    await context.Response.Body.WriteAsync(response);
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
                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                    byte[] response = Encoding.UTF8.GetBytes(challenge);
                    await context.Response.Body.WriteAsync(response);
                    _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: FbMiddleware.VerifyWebhookRequest succesfully verified a webhook");
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    byte[] response = Encoding.UTF8.GetBytes(string.Empty);
                    await context.Response.Body.WriteAsync(response);
                    _logger.LogWarning($"eru.PlatformClients.FacebookMessenger: FbMiddleware.VerifyWebhookRequest got a verify requets with invalid token");
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                byte[] response = Encoding.UTF8.GetBytes(string.Empty);
                await context.Response.Body.WriteAsync(response);
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
                    
                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                    byte[] response = Encoding.UTF8.GetBytes("EVENT_RECEIVED");
                    await context.Response.Body.WriteAsync(response);
                    _logger.LogInformation("eru.PlatformClients.FacebookMessenger: FbMiddleware.HandleWebhookEvent successfully processed an event");
                }
                else
                {
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    byte[] response = Encoding.UTF8.GetBytes(string.Empty);
                    await context.Response.Body.WriteAsync(response);
                    _logger.LogWarning($"eru.PlatformClients.FacebookMessenger: FbMiddleware.HandleWebhookEvent got an unsupported event");
                }
            }
            catch
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                byte[] response = Encoding.UTF8.GetBytes(string.Empty);
                await context.Response.Body.WriteAsync(response);
                _logger.LogError($"eru.PlatformClients.FacebookMessenger: FbMiddleware.HandleWebhookEvent couldn't process event");
            }
        }
    }
}