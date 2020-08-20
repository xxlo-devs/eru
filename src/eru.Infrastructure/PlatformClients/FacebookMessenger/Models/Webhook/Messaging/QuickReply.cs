using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messaging
{
    public class QuickReply
    {
        [JsonPropertyName("payload")]
        public string Payload { get; set; }
    }
}