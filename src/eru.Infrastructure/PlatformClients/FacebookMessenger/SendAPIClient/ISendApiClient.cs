using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient
{
    public interface ISendApiClient
    {
        public Task Send(SendRequest request);
    }
}