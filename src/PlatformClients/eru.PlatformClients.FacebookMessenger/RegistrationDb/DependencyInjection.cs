using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eru.PlatformClients.FacebookMessenger.RegistrationDb
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFacebookMessengerRegistrationDatabase(this IServiceCollection services)
        {
            services.AddScoped<IRegistrationDbContext>(provider => provider.GetService<RegistrationDbContext>());
            services.AddDbContext<RegistrationDbContext>(options => { options.UseInMemoryDatabase($"eru.Infrastructure.PlatformClients.FacebookMessenger"); });
            
            return services;
        }
    }
}