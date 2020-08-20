using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

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
                    throw new NotImplementedException();
                    break;

                case "POST":
                    throw new NotImplementedException();
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    await context.Response.WriteAsync(string.Empty);
                    break;
            }
        }
    }
}
