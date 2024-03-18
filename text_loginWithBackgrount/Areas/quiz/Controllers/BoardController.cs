using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.quiz.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("quiz")]
    public class BoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
