using System.Text.Json.Serialization;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages.Properties;

namespace eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages
{
    public class Message
    {
        [JsonPropertyName("mid")]
        public string Mid { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("quick_reply")]
        public QuickReply QuickReply { get; set; }
    }
}