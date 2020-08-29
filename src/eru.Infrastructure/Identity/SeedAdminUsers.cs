using System.Linq;
using System.Threading.Tasks;
using eru.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eru.Infrastructure.Identity
{
    public class SeedAdminUsers : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var userManager = context.RequestServices.GetService<UserManager<User>>();
            if (!await userManager.Users.AnyAsync())
            {
                var config = context.RequestServices.GetService<IConfiguration>();
                var users = config
                    .GetSection("Admins")
                    .GetChildren()
                    .Select(x=>x.Get<ConfigUser>());
                foreach (var user in users)
                {
                    await userManager.CreateAsync(new User()
                    {
                        Username = user.Username
                    }, user.Password);
                }
            }

            await next.Invoke(context);
        }
    }
}