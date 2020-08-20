using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI.Properties
{
    public class Recipient
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}