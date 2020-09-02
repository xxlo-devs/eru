using System.Threading.Tasks;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.Paging
{
    public interface IPagingMessageHandler
    {
        public Task ShowPreviousPage(string uid);
        public Task ShowNextPage(string uid);
    }
}