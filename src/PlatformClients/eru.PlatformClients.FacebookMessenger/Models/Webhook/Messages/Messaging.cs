using System.Text.Json.Serialization;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Properties;

namespace eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages
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