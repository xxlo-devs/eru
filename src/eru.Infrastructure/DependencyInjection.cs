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
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("eru"));
            }
            else
            {
                //TODO add option from configuration
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=eru.db"));
            }

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            
            services.AddTransient<ISubstitutionsPlanXmlParser, SubstitutionsPlanXmlParser>();
            return services;
        }
    }
}