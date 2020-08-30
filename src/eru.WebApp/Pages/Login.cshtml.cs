using System.Threading.Tasks;
using eru.Domain.Entity;
using Hangfire.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace eru.WebApp.Pages
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class Login : PageModel
    {
        private readonly ILogger<Login> _logger;
        private readonly SignInManager<User> _signInManager;

        public Login(ILogger<Login> logger, SignInManager<User> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        [BindProperty]
        public LoginModel Model { get; set; }
        
        public async Task<RedirectResult> OnPost()
        {
            await _signInManager.PasswordSignInAsync(Model.Username, Model.Password, false, true);
            
            if (HttpContext.Request.Query.TryGetValue("ReturnUrl", out var redirect))
            {
                if (redirect.Count > 0)
                    return Redirect(redirect[0]);
            }

            return RedirectPermanent("/admin");
        }
    }
}