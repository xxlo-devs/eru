using eru.Application.Common.Interfaces;
using eru.Infrastructure.XmlParsing;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<ISubstitutionsPlanXmlParser, SubstitutionsPlanXmlParser>();
            return services;
        }
    }
}