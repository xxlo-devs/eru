using Microsoft.AspNetCore.Mvc;

namespace eru.WebApp.Areas.AdminDashboard.Controllers
{
    [Route("admin")]
    [Area("AdminDashboard")]
    public class AdminDashboardController : Controller
    {
        public IActionResult Dashboard()
            => View();
    }
}