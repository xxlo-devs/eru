using System.Text.Json.Serialization;

namespace eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties
{
    public class QuickReply
    {
        [JsonPropertyName("payload")]
        public string Payload { get; set; }
    }
}