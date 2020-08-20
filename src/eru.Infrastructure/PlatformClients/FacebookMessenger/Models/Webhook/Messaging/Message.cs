using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messaging
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