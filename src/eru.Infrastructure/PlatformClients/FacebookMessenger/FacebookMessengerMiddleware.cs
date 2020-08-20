using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerMiddleware : IMiddleware
    {
        private readonly FacebookMessengerPlatformClient _platformClient;
        private readonly IConfiguration _configuration;

        public FacebookMessengerMiddleware(FacebookMessengerPlatformClient platformClient, IConfiguration configuration)
        {
            _platformClient = platformClient;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            switch (context.Request.Method)
            {
                case "GET":
                    await ValidateFacebookRequest(context);
                    break;

                case "POST":
                    await HandleWebhook(context);
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    await context.Response.WriteAsync(string.Empty);
                    break;
            }
        }

        private async Task ValidateFacebookRequest(HttpContext context)
        {
            string mode = context.Request.Query["hub.mode"];
            string token = context.Request.Query["hub.verify_token"];
            string challenge = context.Request.Query["hub.challenge"];

            if (mode != null && token != null && challenge != null)
            {
                if (mode == "subscribe" && token == _configuration["PlatformClients:FacebookMessenger:VerifyToken"])
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync(challenge);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync(string.Empty);
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(string.Empty);
            }
        }

        private async Task HandleWebhook(HttpContext context)
        {
            try
            {
                var webhook = await JsonSerializer.DeserializeAsync<Event>(context.Request.Body);

                if (webhook.Subscription == "page")
                {
                    foreach (var x in webhook.Entry)
                    {
                        await _platformClient.HandleIncomingMessage(x.Messaging.FirstOrDefault());
                    }

                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                    await context.Response.WriteAsync("EVENT_RECEIVED");
                }
                else
                {
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    await context.Response.WriteAsync(string.Empty);
                }
            }
            catch
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(string.Empty);
                return;
            }
        }
    }
}
