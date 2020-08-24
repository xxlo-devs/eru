using eru.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
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

            return services;
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