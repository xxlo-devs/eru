using System;
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
        private IHttpClientFactory SetupHttpClientFactory(HttpStatusCode responseCode, string responseBody, string expectedRequestBody)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async (HttpRequestMessage message, CancellationToken cancellationToken) =>
                {
                    var content = await message.Content.ReadAsStringAsync();
                    content.Should().BeEquivalentTo(expectedRequestBody);
                    
                    return new HttpResponseMessage(responseCode)
                    {
                        Content = new StringContent(responseBody)
                    };
                });

            var clientMock = new HttpClient(handlerMock.Object) {BaseAddress = new Uri("https://example.com")};
            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(x => x.CreateClient(FacebookMessengerPlatformClient.PId)).Returns(clientMock);

            return factoryMock.Object;
        }
        
        [Fact]
        public async void ShouldSendRequestToFacebookCorrectly()
        {
            var factory = SetupHttpClientFactory(HttpStatusCode.OK,
                "{\"recipient_id\":\"sample-subscriber\",\"message_id\":\"sample-message-id\"}",
                "{\"messaging_type\":\"RESPONSE\",\"recipient\":{\"id\":\"sample-subscriber\"},\"message\":{\"text\":\"hello, world!\"}}"
            );
            
            var request = new SendRequest("sample-subscriber", new Message("hello, world!"));
            
            var apiClient = new SendApiClient(factory, MockBuilder.BuildFakeConfiguration(), MockBuilder.BuildFakeLogger<SendApiClient>());
            await apiClient.Send(request);
        }

        [Fact]
        public async void ShouldHandleErrorWhileSendingRequestToFacebookCorrectly()
        {
            var factory = SetupHttpClientFactory(HttpStatusCode.BadRequest,
                "{\"error\":{\"message\":\"Invalid OAuth access token.\",\"type\":\"OAuthException\",\"code\":190,\"error_subcode\":1234567,\"fbtrace_id\":\"traceid\"}}",
                "{\"messaging_type\":\"RESPONSE\",\"recipient\":{\"id\":\"sample-subscriber\"},\"message\":{\"text\":\"hello, world!\"}}"
                );
            
            var request = new SendRequest("sample-subscriber", new Message("hello, world!"));
            
            var apiClient = new SendApiClient(factory, MockBuilder.BuildFakeConfiguration(), MockBuilder.BuildFakeLogger<SendApiClient>());

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