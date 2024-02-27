using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Class_system_Backstage_pj.Areas.class_discuss.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("class_discuss")]
    public class class_discussController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
