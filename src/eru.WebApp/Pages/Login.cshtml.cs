using System.Threading.Tasks;
using eru.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eru.WebApp.Pages
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    public class Login : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public Login(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public LoginModel Model { get; set; }
        
        public async Task<RedirectResult> OnPostAsync()
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