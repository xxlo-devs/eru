using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Exceptions;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.SendAPIClient
{
    public class SendApiClientTests
    {
        [Fact]
        public async void ShouldSendRequestToFacebookCorrectly()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async (HttpRequestMessage message, CancellationToken cancellationToken) =>
                {
                    var content = await message.Content.ReadAsStringAsync();
                    content.Should().BeEquivalentTo("{\"messaging_type\":\"RESPONSE\",\"recipient\":{\"id\":\"sample-subscriber\"},\"message\":{\"text\":\"hello, world!\"}}");
                    
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"recipient_id\":\"sample-subscriber\",\"message_id\":\"sample-message-id\"}")
                    };
                });

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(x => x.CreateClient(string.Empty)).Returns(new HttpClient(handlerMock.Object));
            
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] {new KeyValuePair<string, string>("PlatformClients:FacebookMessenger:AccessToken", "sample-access-token")}).Build();
            
            var request = new SendRequest("sample-subscriber", new Message("hello, world!"));
            
            var apiClient = new SendApiClient(factoryMock.Object, config, new Mock<ILogger<SendApiClient>>().Object);
            await apiClient.Send(request);
        }

        [Fact]
        public async void ShouldHandleErrorWhileSendingRequestToFacebookCorrectly()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async (HttpRequestMessage message, CancellationToken cancellationToken) =>
                {
                    var content = await message.Content.ReadAsStringAsync();
                    content.Should().BeEquivalentTo("{\"messaging_type\":\"RESPONSE\",\"recipient\":{\"id\":\"sample-subscriber\"},\"message\":{\"text\":\"hello, world!\"}}");
                    
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("{\"error\":{\"message\":\"Invalid OAuth access token.\",\"type\":\"OAuthException\",\"code\":190,\"error_subcode\":1234567,\"fbtrace_id\":\"traceid\"}}")
                    };
                });

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(x => x.CreateClient(string.Empty)).Returns(new HttpClient(handlerMock.Object));
            
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] {new KeyValuePair<string, string>("PlatformClients:FacebookMessenger:AccessToken", "sample-access-token")}).Build();
            
            var request = new SendRequest("sample-subscriber", new Message("hello, world!"));
            
            var apiClient = new SendApiClient(factoryMock.Object, config, new Mock<ILogger<SendApiClient>>().Object);

            try
            {
                await apiClient.Send(request);
            }
            catch (MessageSendingException exception)
            {
                exception.Message.Should().BeEquivalentTo("An exception was thrown while sending a message: Facebook Messenger GraphQL Endpoint returned non-success HTTP Status Code");
            }
        }
    }
}