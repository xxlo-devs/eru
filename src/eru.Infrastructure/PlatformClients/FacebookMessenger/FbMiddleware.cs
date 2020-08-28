using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class FbMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageHandler _messageHandler;

        public FbMiddleware(IConfiguration configuration, IMessageHandler messageHandler)
        {
            _configuration = configuration;
            _messageHandler = messageHandler;
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
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    byte[] response = Encoding.UTF8.GetBytes(string.Empty);
                    await context.Response.Body.WriteAsync(response);
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                byte[] response = Encoding.UTF8.GetBytes(string.Empty);
                await context.Response.Body.WriteAsync(response);
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
                }
                else
                {
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    byte[] response = Encoding.UTF8.GetBytes(string.Empty);
                    await context.Response.Body.WriteAsync(response);
                }
            }
            catch
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                byte[] response = Encoding.UTF8.GetBytes(string.Empty);
                await context.Response.Body.WriteAsync(response);
            }
        }
    }
}