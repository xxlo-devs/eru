using eru.Application.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
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
                case "postgresql":
                    SetupPostgresDbContext(services, configuration);
                    break;
                default:
                    throw new DatabaseSettingsException();
            }

            return services;
        }

        public static IApplicationBuilder UseDatabase(this IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetService<IConfiguration>();
            if (config.GetValue<bool>("Database:AutomaticallyMigrate"))
            {
                var dbContext = app.ApplicationServices.CreateScope().ServiceProvider.GetService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }
            return app;
        }

        private static void SetupSqliteDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("Database:ConnectionString");
            if(string.IsNullOrEmpty(connectionString)) throw new DatabaseSettingsException();
            var dbContextOptionsBuilder = new DbContextOptionsBuilder()
                .UseSqlite(connectionString, y =>
                {
                    y.MigrationsAssembly("eru.Infrastructure");
                });
            using var dbContext = new DbContext(dbContextOptionsBuilder.Options);
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString, y =>
            {
                y.MigrationsAssembly("eru.Infrastructure");
            }));
        }
        
        
        private static void SetupPostgresDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("Database:ConnectionString");
            if(string.IsNullOrEmpty(connectionString)) throw new DatabaseSettingsException();
            var dbContextOptionsBuilder = new DbContextOptionsBuilder()
                .UseNpgsql(connectionString, y =>
                {
                    y.MigrationsAssembly("eru.Infrastructure");
                });
            using var dbContext = new DbContext(dbContextOptionsBuilder.Options);
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString, y =>
            {
                y.MigrationsAssembly("eru.Infrastructure");
            }));
        }
    }
}