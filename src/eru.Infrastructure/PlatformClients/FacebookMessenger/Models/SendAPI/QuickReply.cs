﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI.Static;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI
{
    public class QuickReply
    {
        public QuickReply(string title, string payload)
        {
            ContentType = QuickReplyContentTypes.Text;
            Title = title;
            Payload = payload;
        }

        public QuickReply(string title, string payload, string imageUrl)
        {
            ContentType = QuickReplyContentTypes.Text;
            Title = title;
            Payload = payload;
            ImageUrl = imageUrl;
        }

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
