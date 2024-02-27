using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Class_system_Backstage_pj.Areas.course_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("course_management")]
    public class courseHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
    }
}
