using System.Linq;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
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

        public static IApplicationBuilder UseConfiguredHangfire(this IApplicationBuilder app,
            IConfiguration configuration)
        {
            var users = configuration
                .GetSection("HangfireDashboardUsers")
                .GetChildren()
                .Select(x=>x.Get<HangfireUser>())
                .Select(x => new BasicAuthAuthorizationUser
                {
                    Login = x.Username,
                    PasswordClear = x.Password
                });
            
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization = new []{ new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    LoginCaseSensitive = true,
                    RequireSsl = false,
                    SslRedirect = false,
                    Users = users
                }) },
                AppPath = "/admin"
            });
            
            return app;
        }
    }
}