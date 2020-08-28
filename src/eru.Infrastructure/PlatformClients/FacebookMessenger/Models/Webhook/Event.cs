using System.Collections.Generic;
using System.Text.Json.Serialization;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook
{
    public class Event
    {
        [JsonPropertyName("object")]
        public string Subscription { get; set; }

        [JsonPropertyName("entry")]
        public IEnumerable<Entry> Entry { get; set; }
    }
}