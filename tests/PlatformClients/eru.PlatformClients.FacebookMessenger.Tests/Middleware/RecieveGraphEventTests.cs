using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using HttpMethods = eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Static.HttpMethods;

namespace eru.PlatformClients.FacebookMessenger.Tests.Middleware
{
    public class RecieveGraphEventTests
    {
        private static HttpContext BuildHttpContext(string request)
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request.Method).Returns(HttpMethods.Post);
            httpContext.SetupProperty(x => x.Request.Body, new MemoryStream(Encoding.UTF8.GetBytes(request)));
            httpContext.SetupProperty(x => x.Response.Body, new MemoryStream());
            httpContext.SetupProperty(x => x.Response.StatusCode);

            return httpContext.Object;
        }
        
        private static async Task<string> GetStringBody(Stream str)
        {
            str.Position = 0;
            var reader = new StreamReader(str);
            return await reader.ReadToEndAsync();
        }
            
        [Fact]
        public async void CanRecieveGraphEvent()
        {
            var messageHandler = new Mock<IMessageHandler>();
            var middleware = new FbMiddleware(MockBuilder.BuildFakeConfiguration(), messageHandler.Object, MockBuilder.BuildFakeLogger<FbMiddleware>());
            
            var context = BuildHttpContext("{\"object\":\"page\",\"entry\":[{\"messaging\":[{\"sender\":{\"id\":\"<PSID>\"},\"recipient\":{\"id\":\"<PAGE_ID>\"},\"timestamp\":123456789,\"message\":{\"mid\":\"mid.1457764197618:41d102a3e1ae206a38\",\"text\":\"hello, world!\"}}]}]}");
            
            await middleware.InvokeAsync(context, requestDelegateContext => Task.CompletedTask);
            var content = await GetStringBody(context.Response.Body);
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            content.Should().Be("EVENT_RECEIVED");
        }

        [Fact]
        public async void DoesWebhookReturnNotFoundWhenSubscriptionTargetIsUnknown()
        {
            var messageHandler = new Mock<IMessageHandler>();
            var middleware = new FbMiddleware(MockBuilder.BuildFakeConfiguration(), messageHandler.Object, MockBuilder.BuildFakeLogger<FbMiddleware>());
            
            var context = BuildHttpContext("{\"object\":\"unknown\"}");
            
            await middleware.InvokeAsync(context, requestDelegateContext => Task.CompletedTask);
            var content = await GetStringBody(context.Response.Body);
                
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
            content.Should().BeEmpty();
        }

        [Fact]
        public async void DoesWebhookReturnBadRequestWhenRequestIsInvalid()
        {
            var messageHandler = new Mock<IMessageHandler>();
            var middleware = new FbMiddleware(MockBuilder.BuildFakeConfiguration(), messageHandler.Object, MockBuilder.BuildFakeLogger<FbMiddleware>());
            
            var context = BuildHttpContext("{\"object\": \"page\", \"entry\": [{\"messaging\": [{\"message\": \"TEST_MESSAGE\"}]}]}");
            
            await middleware.InvokeAsync(context, requestDelegateContext => Task.CompletedTask);
            var content = await GetStringBody(context.Response.Body);
            
            context.Response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            content.Should().BeEmpty();
        }
    }
}