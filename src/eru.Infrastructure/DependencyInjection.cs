using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using eru.Application.Common.Exceptions;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.Hangfire;
using eru.Infrastructure.Persistence;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MemoryStorage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            switch (configuration.GetValue<string>("Database:Type")?.ToLower() ?? "inmemory")
            {
                case "inmemory":
                    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("eru"));
                    break;
                case "sqlite":
                    SetupSqliteDbContext(services, configuration);
                    break;
                default:
                    throw new DatabaseSettingsException();
            }
          
            services.AddHangfire(conf =>
            {
                conf
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseColouredConsoleLogProvider()
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseMemoryStorage();
            });
            services.AddHangfireServer();
            
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            
            services.AddTransient<IStopwatch, Stopwatch.Stopwatch>();
            services.AddTransient<IBackgroundExecutor, HangfireExecutror>();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            return services;
        }
        
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IConfiguration configuration)
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
            return app.UseHangfireDashboard("/jobs", new DashboardOptions
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
        }

        private static void SetupSqliteDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("Database:ConnectionString");
            if(string.IsNullOrEmpty(connectionString)) throw new DatabaseSettingsException();
            var dbContextOptionsBuilder = new DbContextOptionsBuilder()
                .UseSqlite(connectionString);
            using var dbContext = new DbContext(dbContextOptionsBuilder.Options);
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
        }
    }
}