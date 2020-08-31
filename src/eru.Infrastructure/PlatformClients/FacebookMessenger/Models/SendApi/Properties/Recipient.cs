﻿using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi.Properties
{
    public class Recipient
    {
        public Recipient(string id)
        {
            Id = id;
        }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}