using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using eru.Application.Common.Exceptions;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.SendAPIClient
{
    public class SendApiClient : ISendApiClient
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SendApiClient> _logger;

        public SendApiClient(IHttpClientFactory factory, IConfiguration configuration, ILogger<SendApiClient> logger)
        {
            _client = factory.CreateClient(FacebookMessengerPlatformClient.PId);
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task Send(SendRequest request)
        {
            var accessToken = $"?access_token={_configuration["PlatformClients:FacebookMessenger:AccessToken"]}";
            
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions {IgnoreNullValues = true});
            var content = new StringContent(json, Encoding.Default, "application/json");

            _logger.LogTrace(
                "Facebook Messenger Send API Client: trying to send a message (message: {json}) to URL: {_client.BaseAddress}{accessToken}",
                json, _client.BaseAddress, accessToken);
            
            var httpRequest = await _client.PostAsync(accessToken, content);
            var httpResponse = await httpRequest.Content.ReadAsStringAsync();
            
            if (!httpRequest.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Facebook Messenger Send API Client: there was an error while sending request to Facebook (request: {httpRequest}, error body: {httpResponse})",
                    httpRequest, httpResponse);
                throw new MessageSendingException("Facebook Messenger GraphQL Endpoint returned non-success HTTP Status Code");
            }
            else
            {
                _logger.LogInformation(
                    "FacebookMessenger Send API Client: successfully send a GraphQL Send API Request to Facebook (HTTP Request: {httpRequest})",
                    httpRequest);
            }
        }
    }
}