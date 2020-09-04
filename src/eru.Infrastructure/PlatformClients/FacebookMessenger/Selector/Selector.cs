using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Selector
{
    public class Selector : ISelector
    {
        public IEnumerable<QuickReply> GetSelector(Dictionary<string, string> items, int page, Type payloadType)
        {
            var offset = page * 10;
            var replies = new List<QuickReply>();

            var buttons =items.Skip(offset).Take(10);

            foreach (var x in buttons)
            {
                replies.Add(new QuickReply(x.Key, x.Value));
            }

            if (page > 0)
            {
                replies.Add(new QuickReply("<-", new Payload(payloadType, page - 1).ToJson()));
            }

            if (items.Count() - offset - 10 > 0)
            {
                replies.Add(new QuickReply("->", new Payload(payloadType, page + 1).ToJson()));
            }

            replies.Add(new QuickReply("Cancel", new Payload(Type.Cancel).ToJson()));

            return replies;
        }
    }
}