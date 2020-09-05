using System.Text.Json.Serialization;
using eru.PlatformClients.FacebookMessenger.Models.SendApi.Properties;
using eru.PlatformClients.FacebookMessenger.Models.SendApi.Static;

namespace eru.PlatformClients.FacebookMessenger.Models.SendApi
{
    public class SendRequest
    {
        public SendRequest(string uid, Message message)
        {
            Type = MessagingTypes.Response;
            Recipient = new Recipient(uid);
            Message = message;
        }

        public SendRequest(string uid, Message message, string tag)
        {
            Type = MessagingTypes.MessageTag;
            Tag = tag;
            Recipient = new Recipient(uid);
            Message = message;
        }

        public SendRequest(string type, string tag, string uid, Message message)
        {
            Type = type;
            Tag = tag;
            Recipient = new Recipient(uid);
            Message = message;
        }

        [JsonPropertyName("messaging_type")]
        public string Type { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("recipient")]
        public Recipient Recipient { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }
}