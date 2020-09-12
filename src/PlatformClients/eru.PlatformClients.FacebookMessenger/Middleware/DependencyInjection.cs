using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.Middleware
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFbMiddleware(this IServiceCollection services)
        {
            services.AddTransient<FbMiddleware>();

            return services;
        }

        public static IApplicationBuilder UseFbMiddleware(this IApplicationBuilder app)
        {
            app.Map("/fbwebhook", x => x.UseMiddleware<FbMiddleware>());

            return app;
        }
    }
}