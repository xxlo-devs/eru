using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messaging.Properties;
using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messaging
{
    public class Messaging
    {
        [JsonPropertyName("sender")]
        public Sender Sender { get; set; }

        [JsonPropertyName("recipient")]
        public Recipient Recipient {get; set;}
        
        [JsonPropertyName("timestamp")]
        public ulong Timestamp { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }
}