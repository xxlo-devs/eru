using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using eru.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace eru.WebApp.Pages
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
    
    public class Login : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IStringLocalizer<Login> _localizer;

        public Login(SignInManager<User> signInManager, IStringLocalizer<Login> localizer)
        {
            _signInManager = signInManager;
            _localizer = localizer;
        }

        [BindProperty]
        public LoginModel Model { get; set; }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(Model.Username, Model.Password, false, true);
            if (result.Succeeded)
            {
                if (HttpContext.Request.Query.TryGetValue("ReturnUrl", out var redirect))
                {
                    if (redirect.Count > 0)
                        return Redirect(redirect[0]);
                }

                return RedirectPermanent("admin");   
            }
            else
            {
                ModelState.AddModelError("cannot-login", _localizer["cannot-log-in"]);
                return Page();
            }
        }
    }
}