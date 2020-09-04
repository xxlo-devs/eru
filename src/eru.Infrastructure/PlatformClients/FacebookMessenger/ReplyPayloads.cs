using System.Text.Json;
using System.Text.Json.Serialization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class Payload
    {
        public Payload(Type type)
        {
            Type = type;
        }
        public Payload(Type type, string id)
        {
            Type = type;
            Id = id;
        }

        public Payload(Type type, int page)
        {
            Type = type;
            Page = page;
        }
        public Type Type { get; set; }
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

    public enum Type
    {
        Cancel,
        Subscribe, 
        Lang,
        Year,
        Class
    }
    
    // public static class ReplyPayloads
    // {
    //     public const string CancelPayload = "cancel";
    //     public const string SubscribePayload = "subscribe";
    //     public const string LangPrefix = "lang:";
    //     public const string ClassPrefix = "class:";
    //     public const string YearPrefix = "year:";
    //     public const string NextPage = "nextpage";
    //     public const string PreviousPage = "previouspage";
    // }
}