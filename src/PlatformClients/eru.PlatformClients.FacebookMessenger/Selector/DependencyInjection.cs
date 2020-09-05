using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.Selector
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSelector(this IServiceCollection services)
            => services.AddTransient<ISelector, Selector>();
    }
}