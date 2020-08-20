namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class ReplyPayloads
    {
        public const string CancelPayload = "cancel";
        public const string SubscribePayload = "subscribe";
        public static string ClassPayload(string name) => @"class_{name}";
        public static string LanguagePayload(string cultureName) => $@"lang_{cultureName}";
        public const string NextPage = "nextpage";
        public const string PreviousPage = "previouspage";
    }
}