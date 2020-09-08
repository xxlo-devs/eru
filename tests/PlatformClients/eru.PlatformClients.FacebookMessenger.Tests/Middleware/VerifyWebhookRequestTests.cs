using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.Middleware
{
    public class VerifyWebhookRequestTests
    {
        private readonly IConfiguration _configuration;

        public VerifyWebhookRequestTests()
        {
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("PlatformClients:FacebookMessenger:VerifyToken", "sample-verify-token")
            })
                .Build();
        }
        
        private HttpContext BuildHttpContext(Dictionary<string, StringValues> queries)
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request.Method).Returns("GET");
            httpContext.SetupProperty(x => x.Request.Query,
                new QueryCollection(queries)
            );
            httpContext.SetupProperty(x => x.Response.Body, new MemoryStream());
            httpContext.SetupProperty(x => x.Response.StatusCode);

            return httpContext.Object;
        }
        
        private async Task<string> GetStringBody(Stream str)
        {
            str.Position = 0;
            StreamReader reader = new StreamReader(str);
            return await reader.ReadToEndAsync();
        }
        
        [Fact]
        public async void CanVerifyWebhook()
        {
            var logger = new Mock<ILogger<FbMiddleware>>();
            var messageHandler = new Mock<MessageHandler<IncomingMessageHandler>>(new Mock<ILogger<IncomingMessageHandler>>().Object);
            var middleware = new FbMiddleware(_configuration, messageHandler.Object, logger.Object);
            var context = BuildHttpContext(new Dictionary<string, StringValues>
            {
                {"hub.mode", "subscribe"}, 
                {"hub.verify_token", "sample-verify-token"},
                {"hub.challenge", "sample-challenge"}
            });
            
            await middleware.InvokeAsync(context, context => throw new NotImplementedException());
            var content = await GetStringBody(context.Response.Body);
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            content.Should().Be("sample-challenge");
        }

        [Fact]
        public async void CannotVerifyWebhookWithInvalidMode()
        {
            var logger = new Mock<ILogger<FbMiddleware>>();
            var messageHandler = new Mock<MessageHandler<IncomingMessageHandler>>(new Mock<ILogger<IncomingMessageHandler>>().Object);
            var middleware = new FbMiddleware(_configuration, messageHandler.Object, logger.Object);
            var context = BuildHttpContext(new Dictionary<string, StringValues>
            {
                {"hub.mode", "invalid-mode"}, 
                {"hub.verify_token", "sample-verify-token"},
                {"hub.challenge", "sample-challenge"}
            });
            
            await middleware.InvokeAsync(context, context => throw new NotImplementedException());
            var content = await GetStringBody(context.Response.Body);
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.Forbidden);
            content.Should().BeEmpty();
        }

        [Fact]
        public async void CannotVerifyWebhookWithInvalidToken()
        {
            var logger = new Mock<ILogger<FbMiddleware>>();
            var messageHandler = new Mock<MessageHandler<IncomingMessageHandler>>(new Mock<ILogger<IncomingMessageHandler>>().Object);
            var middleware = new FbMiddleware(_configuration, messageHandler.Object, logger.Object);
            var context = BuildHttpContext(new Dictionary<string, StringValues>
            {
                {"hub.mode", "subscribe"}, 
                {"hub.verify_token", "invalid-verify-token"},
                {"hub.challenge", "sample-challenge"}
            });
            
            await middleware.InvokeAsync(context, context => throw new NotImplementedException());
            var content = await GetStringBody(context.Response.Body);
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.Forbidden);
            content.Should().BeEmpty();
        }

        [Fact]
        public async void CannotVerifyWebhookWithoutChallenge()
        {
            var logger = new Mock<ILogger<FbMiddleware>>();
            var messageHandler = new Mock<MessageHandler<IncomingMessageHandler>>(new Mock<ILogger<IncomingMessageHandler>>().Object);
            var middleware = new FbMiddleware(_configuration, messageHandler.Object, logger.Object);
            var context = BuildHttpContext(new Dictionary<string, StringValues>
            {
                {"hub.mode", "subscribe"}, 
                {"hub.verify_token", "sample-verify-token"}
            });
            
            await middleware.InvokeAsync(context, context => throw new NotImplementedException());
            var content = await GetStringBody(context.Response.Body);
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            content.Should().BeEmpty();
        }
    }
}