using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi
{
    public class Message
    {
        public Message(string text)
        {
            Text = text;
        }

        public Message(string text, IEnumerable<QuickReply> replies)
        {
            Text = text;
            QuickReplies = replies;
        }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("quick_replies")]
        public IEnumerable<QuickReply> QuickReplies { get; set; }
    }
}