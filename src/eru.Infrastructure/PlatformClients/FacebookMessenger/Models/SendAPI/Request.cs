using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI.Properties;
using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI
{
    public class Request
    {
        [JsonPropertyName("messaging_type")]
        public string Type { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("recipient")]
        public Recipient Recipient { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }
}