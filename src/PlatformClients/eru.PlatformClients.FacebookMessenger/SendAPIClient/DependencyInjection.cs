using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.SendAPIClient
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSendApiClient(this IServiceCollection services)
            => services.AddHttpClient().AddTransient<ISendApiClient, SendApiClient>();
    }
}