using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.Middleware
{
    public class NotSupportedRequestTests
    {
        [Fact]
        public async void CannotHandleRequestWithUnsupportedMethod()
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request.Method).Returns("PUT");
            httpContext.SetupProperty(x => x.Response.Body, new MemoryStream());
            httpContext.SetupProperty(x => x.Response.StatusCode);
            var context = httpContext.Object;
            
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] {new KeyValuePair<string, string>("PlatformClients:FacebookMessenger:VerifyToken", "sample-verify-token")}).Build();
            var messageHandler = new Mock<IMessageHandler>();
            var middleware = new FbMiddleware(config, messageHandler.Object);
            
            await middleware.InvokeAsync(context, context => throw new NotImplementedException());
            
            context.Response.Body.Position = 0; StreamReader reader = new StreamReader(context.Response.Body); var content = await reader.ReadToEndAsync();
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.MethodNotAllowed);
            content.Should().BeEmpty();
        }
    }
}