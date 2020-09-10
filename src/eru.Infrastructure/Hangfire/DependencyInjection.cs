using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.Hangfire
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddConfiguredHangfire(this IServiceCollection services)
        {
            services.AddHangfire(conf =>
            {
                conf
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseColouredConsoleLogProvider()
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSerilogLogProvider()
                    .UseMemoryStorage();
            });
            services.AddHangfireServer();
            
            return services;
        }
        
        public class AuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();
                return httpContext.User.Identity.IsAuthenticated;
            }
        }

        public static IApplicationBuilder UseConfiguredHangfire(this IApplicationBuilder app,
            IConfiguration configuration)
        {
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization = new [] {new AuthorizationFilter()},
                AppPath = "admin"
            });
            
            return app;
        }
    }
}