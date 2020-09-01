using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Selector
{
    public interface ISelector
    {
        public IEnumerable<QuickReply> GetSelector(Dictionary<string, string> items, int offset);
    }
}