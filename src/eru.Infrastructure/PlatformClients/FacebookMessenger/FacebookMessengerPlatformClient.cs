using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerPlatformClient : IPlatformClient
    {
        public string PlatformId { get; } = "FacebookMessenger"; 
        
        public Task SendMessage(string id, string content)
        {
            throw new System.NotImplementedException();
        }

        public Task SendMessage(string id, IEnumerable<Substitution> substitutions)
        {
            throw new System.NotImplementedException();
        }
    }
}