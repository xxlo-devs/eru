using System;
using System.Linq;
using System.Reflection;
using eru.Application.Common.Exceptions;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.Persistence;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackgroundJobClient = eru.Infrastructure.Hangfire.BackgroundJobClient;
using IBackgroundJobClient = eru.Application.Common.Interfaces.IBackgroundJobClient;

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
            services.AddTransient<IBackgroundJobClient, BackgroundJobClient>();
            
            LoadAllMessageServices(services);
            
            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            return app.UseHangfireDashboard();
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

        private static void LoadAllMessageServices(IServiceCollection services)
        {
            var messageServices = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x=>x.ExportedTypes)
                .Where(x => typeof(IMessageService).IsAssignableFrom(x) && !x.IsInterface);
            foreach (var messageService in messageServices)
            {
                services.AddTransient(typeof(IMessageService), messageService);
            }
        }
    }
}