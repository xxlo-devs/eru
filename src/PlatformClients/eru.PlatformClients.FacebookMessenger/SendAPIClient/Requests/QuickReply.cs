using System.Text.Json.Serialization;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Static;

namespace eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests
{
    public class QuickReply
    {
        public QuickReply(string title, string payload)
        {
            ContentType = QuickReplyContentTypes.Text;
            Title = title;
            Payload = payload;
        }

        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("payload")]
        public string Payload { get; set; }
    }
}