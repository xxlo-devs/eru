using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand
{
    public class UnsupportedCommandMessageHandler : IUnsupportedCommandMessageHandler
    {
        private readonly ISendApiClient _apiClient;
        
        public UnsupportedCommandMessageHandler(ISendApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        
        public async Task Handle(string uid)
        {
            await _apiClient.Send(new SendRequest(uid,
                new Message("This is not a supported command. If you want to cancel your subscription, just click Cancel. If you want to recieve notifications, just ignore this message.", new[]
                {
                    new QuickReply("Cancel", ReplyPayloads.CancelPayload), 
                })));
        }
    }
}