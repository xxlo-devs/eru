using eru.Application.Common.Exceptions;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.Persistence;
using eru.Infrastructure.XmlParsing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            switch (configuration.GetValue<string>("Database:Type").ToLower())
            {
                case "inmemory":
                    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("eru"));
                    break;
                case "sqlite":
                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(configuration.GetValue<string>("Database:ConnectionString")));
                    break;
                default:
                    throw new DatabaseSettingsException();
            }
            
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            
            services.AddTransient<ISubstitutionsPlanXmlParser, SubstitutionsPlanXmlParser>();
            services.AddTransient<IStopwatch, Stopwatch.Stopwatch>();
            return services;
        }
    }
}