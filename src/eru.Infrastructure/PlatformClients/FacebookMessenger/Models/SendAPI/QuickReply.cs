using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI
{
    public class QuickReply
    {
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("payload")]
        public string Payload { get; set; }
        
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
    }
}
