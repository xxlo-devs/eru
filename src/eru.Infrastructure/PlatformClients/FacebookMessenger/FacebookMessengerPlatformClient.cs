using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerPlatformClient : IPlatformClient
    {
        public string PlatformId { get; } = "FacebookMessenger";
        
        public async Task SendMessage(string id, string content)
        {
            throw new System.NotImplementedException();
        }

        public async Task SendMessage(string id, IEnumerable<Substitution> substitutions)
        {
            throw new System.NotImplementedException();
        }

        public async Task Send()
        {
            throw new System.NotImplementedException();
        }
    }
}