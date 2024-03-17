using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace text_loginWithBackgrount.Areas.quiz.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher, student")]
    [Area("quiz")]
    public class GoController : Controller
    {

        public IActionResult Index()
        {
            var userRole = User?.FindFirstValue(ClaimTypes.Role);

            if (userRole == "teacher")
            {
                return RedirectToAction("BgIndex", "QuizBg");
            }

            return RedirectToAction("StdIndex", "Quiz");
        }
    }
}
