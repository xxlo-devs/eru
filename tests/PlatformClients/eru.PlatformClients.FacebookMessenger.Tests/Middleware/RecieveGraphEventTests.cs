﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.Middleware
{
    public class RecieveGraphEventTests
    {
        private readonly IConfiguration _configuration;

        public RecieveGraphEventTests()
        {
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("PlatformClients:FacebookMessenger:VerifyToken", "sample-verify-token")
            })
                .Build();
        }
        
        private HttpContext BuildHttpContext(string request)
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request.Method).Returns("POST");
            httpContext.SetupProperty(x => x.Request.Body, new MemoryStream(Encoding.UTF8.GetBytes(request)));
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
        public async void CanRecieveGraphEvent()
        {
            var messageHandler = new Mock<IMessageHandler>();
            var logger = new Mock<ILogger<FbMiddleware>>();
            var middleware = new FbMiddleware(_configuration, messageHandler.Object, logger.Object);
            var context = BuildHttpContext("{\"object\":\"page\",\"entry\":[{\"messaging\":[{\"sender\":{\"id\":\"<PSID>\"},\"recipient\":{\"id\":\"<PAGE_ID>\"},\"timestamp\":123456789,\"message\":{\"mid\":\"mid.1457764197618:41d102a3e1ae206a38\",\"text\":\"hello, world!\"}}]}]}");
            
            await middleware.InvokeAsync(context, requestDelegateContext => Task.CompletedTask);
            var content = await GetStringBody(context.Response.Body);
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            content.Should().Be("EVENT_RECEIVED");
        }

        [Fact]
        public async void DoesWebhookReturnNotFoundWhenSubscriptionTargetIsUnknown()
        {
            var logger = new Mock<ILogger<FbMiddleware>>();
            var messageHandler = new Mock<IMessageHandler>();
            var middleware = new FbMiddleware(_configuration, messageHandler.Object, logger.Object);
            var context = BuildHttpContext("{\"object\":\"unknown\"}");
            
            await middleware.InvokeAsync(context, requestDelegateContext => Task.CompletedTask);
            var content = await GetStringBody(context.Response.Body);
                
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
            content.Should().BeEmpty();
        }

        [Fact]
        public async void DoesWebhookReturnBadRequestWhenRequestIsInvalid()
        {
            var logger = new Mock<ILogger<FbMiddleware>>();
            var messageHandler = new Mock<IMessageHandler>();
            var middleware = new FbMiddleware(_configuration, messageHandler.Object, logger.Object);
            var context = BuildHttpContext("{\"object\": \"page\", \"entry\": [{\"messaging\": [{\"message\": \"TEST_MESSAGE\"}]}]}");
            
            await middleware.InvokeAsync(context, requestDelegateContext => Task.CompletedTask);
            var content = await GetStringBody(context.Response.Body);
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            content.Should().BeEmpty();
        }
    }
}