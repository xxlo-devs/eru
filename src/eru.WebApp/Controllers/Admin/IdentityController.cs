using System.Threading.Tasks;
using eru.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace eru.WebApp.Controllers.Admin
{
    public class IdentityController : Controller
    {
        [HttpGet]
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.RequestServices.GetService<SignInManager<User>>().SignOutAsync();
            return RedirectPermanent("login");
        }
    }
}