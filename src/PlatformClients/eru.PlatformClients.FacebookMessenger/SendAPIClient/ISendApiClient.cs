using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;

namespace eru.PlatformClients.FacebookMessenger.SendAPIClient
{
    public interface ISendApiClient
    {
        public Task Send(SendRequest request);
    }
}