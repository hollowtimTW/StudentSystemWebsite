using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Class_system_Backstage_pj.Areas.ordering_system.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("ordering_system")]
    public class ordering_systemController : Controller
    {
        public IActionResult ordering_system_index()
        {
            return View();
        }
    }
}
