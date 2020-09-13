using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eru.PlatformClients.FacebookMessenger.Middleware.Webhook
{
    // Class is instantiated by JSON Deserializer
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Event
    {
        [JsonPropertyName("object")]
        public string Subscription { get; set; }

        [JsonPropertyName("entry")]
        public IEnumerable<Entry> Entry { get; set; }
    }
}