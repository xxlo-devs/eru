using System.Reflection;
using eru.Application.Configuration;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.LoadConfigs(configuration);
            return services;
        }
    }
}