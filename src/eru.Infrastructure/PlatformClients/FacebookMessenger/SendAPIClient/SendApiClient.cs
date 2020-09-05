using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using Microsoft.Extensions.Configuration;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient
{
    public class SendApiClient : ISendApiClient
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public SendApiClient(IConfiguration configuration)
        {
            _client = new HttpClient();
            _configuration = configuration;
        }
        
        public async Task Send(SendRequest request)
        {
            const string graphEndpoint = "https://graph.facebook.com/v8.0/me/messages";
            var accessToken = $"?access_token={_configuration["PlatformClients:FacebookMessenger:AccessToken"]}";
            
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions {IgnoreNullValues = true});
            var content = new StringContent(json, Encoding.Default, "application/json");

            var httpRequest = await _client.PostAsync($"{graphEndpoint}{accessToken}", content);
            var httpResponse = await httpRequest.Content.ReadAsStringAsync();
        }
    }
}