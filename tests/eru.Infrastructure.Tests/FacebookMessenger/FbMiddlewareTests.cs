using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace eru.Infrastructure.Tests.FacebookMessenger
{
    public class FbMiddlewareTests
    {
        [Fact]
        public async void CanVerifyWebhook()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] {new KeyValuePair<string, string>("PlatformClients:FacebookMessenger:VerifyToken", "sample-verify-token")}).Build();
            var messageHandler = new Mock<IMessageHandler>();
            var middleware = new FbMiddleware(config, messageHandler.Object);
            
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request.Method).Returns("GET");
            httpContext.SetupProperty(x => x.Request.Query,
                new QueryCollection(new Dictionary<string, StringValues> { {"hub.mode", "subscribe"}, {"hub.verify_token", "sample-verify-token"}, {"hub.challenge", "sample-challenge"} })
            );
            httpContext.SetupProperty(x => x.Response.Body, new MemoryStream());
            httpContext.SetupProperty(x => x.Response.StatusCode, 0);

            var context = httpContext.Object;
            
            await middleware.InvokeAsync(context, context => throw new NotImplementedException());

            context.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            var bodystream = context.Response.Body; var ms = new MemoryStream(); bodystream.Position = 0; await bodystream.CopyToAsync(ms);
            var content = Encoding.UTF8.GetString(ms.ToArray());
            
            content.Should().Be("sample-challenge");
        }

        [Fact]
        public async void CanSendGraphEvent()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] {new KeyValuePair<string, string>("PlatformClients:FacebookMessenger:VerifyToken", "sample-verify-token")}).Build();

            Messaging msg = new Messaging();
            var messageHandler = new Mock<IMessageHandler>();
            
            messageHandler.Setup(x => x.Handle(It.IsAny<Messaging>())).Callback((Messaging m) =>
            {
                msg = m;
            });

            var middleware = new FbMiddleware(config, messageHandler.Object);
            
            var httpContext = new Mock<HttpContext>();
            string graphRequest = "{\"object\":\"page\",\"entry\":[{\"messaging\":[{\"sender\":{\"id\":\"<PSID>\"},\"recipient\":{\"id\":\"<PAGE_ID>\"},\"timestamp\":123456789,\"message\":{\"mid\":\"mid.1457764197618:41d102a3e1ae206a38\",\"text\":\"hello, world!\"}}]}]}";
            httpContext.Setup(x => x.Request.Method).Returns("POST");
            httpContext.SetupProperty(x => x.Request.Body, new MemoryStream(Encoding.UTF8.GetBytes(graphRequest)));
            httpContext.SetupProperty(x => x.Response.Body, new MemoryStream());
            httpContext.SetupProperty(x => x.Response.StatusCode, 0);

            var context = httpContext.Object;

            await middleware.InvokeAsync(context, context => throw new NotImplementedException());

            context.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            var bodystream = context.Response.Body; var ms = new MemoryStream(); bodystream.Position = 0; await bodystream.CopyToAsync(ms);
            var content = Encoding.UTF8.GetString(ms.ToArray());
            
            content.Should().Be("EVENT_RECEIVED");
            msg.Sender.Id.Should().Be("<PSID>");
            msg.Recipient.Id.Should().Be("<PAGE_ID>");
            msg.Timestamp.Should().Be(123456789);
            msg.Message.Mid.Should().Be("mid.1457764197618:41d102a3e1ae206a38");
            msg.Message.Text.Should().Be("hello, world!");
        }

        [Fact]
        public async void CannotSendInvalidRequest()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] {new KeyValuePair<string, string>("PlatformClients:FacebookMessenger:VerifyToken", "sample-verify-token")}).Build();
            var messageHandler = new Mock<IMessageHandler>();
            
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request.Method).Returns("PUT");
            httpContext.SetupProperty(x => x.Response.Body, new MemoryStream());
            httpContext.SetupProperty(x => x.Response.StatusCode, 0);
            
            var middleware = new FbMiddleware(config, messageHandler.Object);
            var context = httpContext.Object;
            await middleware.InvokeAsync(context, context => throw new NotImplementedException());
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.MethodNotAllowed);
            var stream = new MemoryStream();
            await context.Response.Body.CopyToAsync(stream);
            var content = Encoding.UTF8.GetString(stream.ToArray());
            content.Should().BeEmpty();
        }
    }
}