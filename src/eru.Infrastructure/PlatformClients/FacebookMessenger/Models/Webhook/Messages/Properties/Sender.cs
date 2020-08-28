using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages.Properties
{
    public class Sender
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}