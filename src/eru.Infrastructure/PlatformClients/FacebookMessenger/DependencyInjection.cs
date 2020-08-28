using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFacebookMessenger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<FbMiddleware>();
            
            return services;
        }

        public static IApplicationBuilder UseFacebookMessenger(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.Map("/fbwebhook", x => x.UseMiddleware<FbMiddleware>());
            
            return app;
        }
    }
}