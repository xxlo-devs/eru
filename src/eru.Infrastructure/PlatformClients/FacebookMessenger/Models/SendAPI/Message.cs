using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI
{
    public class Message
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}