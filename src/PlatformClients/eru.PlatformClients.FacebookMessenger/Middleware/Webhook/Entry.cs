using System.Collections.Generic;
using System.Text.Json.Serialization;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.Middleware.Webhook
{ 
    // Class is instantiated by JSON Deserializer
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Entry
    {
        [JsonPropertyName("messaging")]
        public IEnumerable<Messaging> Messaging { get; set; }
    }
}