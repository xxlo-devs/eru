using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook
{
    public class Entry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("time")]
        public ulong Time { get; set; }

        [JsonPropertyName("messaging")]
        public IEnumerable<Messaging.Messaging> Messaging { get; set; }
    }
}