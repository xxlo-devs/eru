using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using eru.Application.Common.Exceptions;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.SendAPIClient
{
    public class SendApiClient : ISendApiClient
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public SendApiClient(IConfiguration configuration, ILogger logger)
        {
            _client = new HttpClient();
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task Send(SendRequest request)
        {
            const string graphEndpoint = "https://graph.facebook.com/v8.0/me/messages";
            var accessToken = $"?access_token={_configuration["PlatformClients:FacebookMessenger:AccessToken"]}";
            
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions {IgnoreNullValues = true});
            var content = new StringContent(json, Encoding.Default, "application/json");

            _logger.LogTrace($"eru.PltaformClients.FacebookMessenger: SendApiClient.Send got a SendRequest (json: {json}) to URL: {graphEndpoint}{accessToken}");
            
            var httpRequest = await _client.PostAsync($"{graphEndpoint}{accessToken}", content);
            var httpResponse = await httpRequest.Content.ReadAsStringAsync();
            
            if (!httpRequest.IsSuccessStatusCode)
            {
                _logger.LogError($"eru.PlatformClients.FacebookMessenger: SendApiClient.Send There was an error while sending request {json} to Facebook");
                _logger.LogError($"eru.PlatformClients.FacebookMessenger: SendApiClient.Send GraphQL Endpoint returned HTTP {httpRequest.StatusCode} Status Code");
                _logger.LogError($"eru.PlatformClients.FacebookMessenger: SendApiClient.Send Error body: {httpResponse}");
                throw new MessageSendingException("Facebook Messenger GraphQL Endpoint returned non-success HTTP Status Code");
            }
            else
            {
                _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: SendApiClient.Send successfully send a GraphQL Send API Request to Facebook (HTTP Status Code: {httpRequest.StatusCode}");
            }
        }
    }
}