﻿using System.IO;
using System.Net;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
            
            var messageHandler = new Mock<IMessageHandler>();
            
            var middleware = new FbMiddleware(MockBuilder.BuildFakeConfiguration(), messageHandler.Object, MockBuilder.BuildFakeLogger<FbMiddleware>());
            await middleware.InvokeAsync(context, requestDelegateContext => Task.CompletedTask);
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.MethodNotAllowed);
            
            context.Response.Body.Position = 0; var reader = new StreamReader(context.Response.Body); var content = await reader.ReadToEndAsync();
            content.Should().BeEmpty();
        }
    }
}