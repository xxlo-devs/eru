using eru.Application.Common.Interfaces;
using eru.Infrastructure.Hangfire;
using eru.Infrastructure.Identity;
using eru.Infrastructure.Persistence;
using eru.Infrastructure.Translation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTranslator(configuration);
            
            services.AddDatabase(configuration);
            
            services.AddIdentity();
            
            services.AddConfiguredHangfire();
            
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            
            services.AddTransient<IStopwatch, Stopwatch.Stopwatch>();
            services.AddTransient<IClassesParser, ClassesParser.ClassesParser>();
            services.AddTransient<IHangfireWrapper, HangfireWrapper>();
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();
            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IConfiguration configuration) =>
            app
                .UseTranslator()
                .UseIdentity()
                .UseConfiguredHangfire(configuration);

        public static IMvcBuilder UseInfrastructure(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder
                .UseTranslator();
        }
    }
}