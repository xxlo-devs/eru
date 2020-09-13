using System.Linq;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;

namespace eru.PlatformClients.FacebookMessenger.Tests
{
    public static class SendRequestComparer
    {
        public static bool IsEquivalentTo(this SendRequest actual, SendRequest expected)
        {
            if (actual.Type != expected.Type) return false;
            if (actual.Tag != expected.Tag) return false;
            if (actual.Recipient.Id != expected.Recipient.Id) return false;
            if (actual.Message.Text != expected.Message.Text) return false;
            if (expected.Message.QuickReplies != null)
            {
                if (actual.Message.QuickReplies.Count() != expected.Message.QuickReplies.Count()) return false;
                if (!actual.Message.QuickReplies.All(x => expected.Message.QuickReplies.Any(y =>
                    y.ContentType == x.ContentType && y.Title == x.Title && y.Payload == x.Payload))) return false;
            }

            return true;
            
        }
    }
}