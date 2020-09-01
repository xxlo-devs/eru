using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Selector
{
    public class Selector : ISelector
    {
        public IEnumerable<QuickReply> GetSelector(Dictionary<string, string> items, int offset)
        {
            var replies = new List<QuickReply>();

            var buttons =items.Skip(offset).Take(10);

            foreach (var x in buttons)
            {
                replies.Add(new QuickReply(x.Key, x.Value));
            }
            
            if(offset > 0)
                replies.Add(new QuickReply("<-", ReplyPayloads.PreviousPage));
            
            if(items.Count() - offset - 10 > 0)
                replies.Add(new QuickReply("->", ReplyPayloads.NextPage));
            
            replies.Add(new QuickReply("Cancel", ReplyPayloads.CancelPayload));

            return replies;
        }
    }
}