using System.Text.Json;
using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload
{
    public class Payload
    {
        public Payload()
        {
            
        }
        public Payload(PayloadType type)
        {
            Type = type;
        }
        public Payload(PayloadType type, string id)
        {
            Type = type;
            Id = id;
        }

        public Payload(PayloadType type, int page)
        {
            Type = type;
            Page = page;
        }
        public PayloadType Type { get; set; }
        public int? Page { get; set; }
        public string Id { get; set; }

        public string ToJson() 
            => JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            });
    }
}