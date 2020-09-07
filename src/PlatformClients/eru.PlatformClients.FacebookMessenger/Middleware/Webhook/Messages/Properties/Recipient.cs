using System.Text.Json.Serialization;

namespace eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Properties
{
    public class Recipient
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}