using eru.Domain.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.Identity
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddDbContext<UserDbContext>()
                .AddIdentityCore<User>()
                .AddUserStore<LocalUserStore>()
                .AddSignInManager();
            services
                .AddTransient<SeedAdminUsers>();
            //IDK Why it does not work without this 'magic' string ¯\_(ツ)_/¯
            services.AddAuthentication("Identity.Application")
                .AddCookie("Identity.Application", options =>
                {
                    options.LoginPath = "/login";
                })
                .AddCookie("Identity.External")
                .AddCookie("Identity.TwoFactorUserId");
            services.AddAuthorization();
            return services;
        }

        public static IApplicationBuilder UseIdentity(this IApplicationBuilder app) =>
            app
                .UseAuthentication()
                .UseAuthorization()
                .UseMiddleware<SeedAdminUsers>();
    }
}