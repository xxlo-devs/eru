using System.Text.Json.Serialization;

namespace eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Properties
{
    public class Recipient
    {
        public Recipient(string id)
        {
            Id = id;
        }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}