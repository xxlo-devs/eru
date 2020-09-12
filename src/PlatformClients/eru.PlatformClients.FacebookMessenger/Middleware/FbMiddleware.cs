using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Static;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HttpMethods = eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Static.HttpMethods;

namespace eru.PlatformClients.FacebookMessenger.Middleware
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
                case HttpMethods.Get:
                    await VerifyWebhookRequest(context);
                    break;

                case HttpMethods.Post:
                    await HandleWebhookEvent(context);
                    break;

                default:
                    _logger.LogWarning("Facebook Messenger Webhook Middleware got a request with not supported method");
                    await context.SendResponse(HttpStatusCode.MethodNotAllowed);
                    break;
            }
        }

        private async Task VerifyWebhookRequest(HttpContext context)
        {
            string mode = context.Request.Query[VerificationParameters.Mode];
            string token = context.Request.Query[VerificationParameters.Token];
            string challenge = context.Request.Query[VerificationParameters.Challenge];

            if (mode != null && token != null && challenge != null)
            {
                if (mode == VerificationMode.Subscribe && token == _configuration["PlatformClients:FacebookMessenger:VerifyToken"])
                {
                    await context.SendOkResponse(challenge);
                    _logger.LogInformation("Facebook Messenger Webhook Middleware succesfully verified a webhook");
                }
                else
                {
                    await context.SendResponse(HttpStatusCode.Forbidden);
                    _logger.LogWarning("Facebook Messenger Webhook Middleware got a verify requets with invalid token");
                }
            }
            else
            {
                await context.SendResponse(HttpStatusCode.BadRequest);
                _logger.LogWarning("Facebook Messenger Webhook Middleware got an invalid verify request");
            }
        }

        private async Task HandleWebhookEvent(HttpContext context)
        {
            try
            {
                var webhook = await JsonSerializer.DeserializeAsync<Event>(context.Request.Body);

                if (webhook.Subscription == Subscriptions.PageSubscription)
                {
                    foreach (var x in webhook.Entry)
                    {
                        await _messageHandler.Handle(x.Messaging.First());
                    }
                    
                    await context.SendOkResponse(Responses.EventRecieved);
                    _logger.LogInformation("Facebook Messenger Webhook Middleware successfully processed an event");
                }
                else
                {
                    await context.SendResponse(HttpStatusCode.NotFound);
                    _logger.LogWarning("Facebook Messenger Webhook Middleware got an unsupported event");
                }
            }
            catch
            {
                await context.SendResponse(HttpStatusCode.BadRequest);
                _logger.LogError("Facebook Messenger Webhook Middleware couldn't process event");
            }
        }
    }
}