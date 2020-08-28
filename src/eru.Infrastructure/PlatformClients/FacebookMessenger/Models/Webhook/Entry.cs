using System.Collections.Generic;
using System.Text.Json.Serialization;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook
{
    public class Entry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("time")]
        public ulong Time { get; set; }

        [JsonPropertyName("messaging")]
        public IEnumerable<Messaging> Messaging { get; set; }
    }
}