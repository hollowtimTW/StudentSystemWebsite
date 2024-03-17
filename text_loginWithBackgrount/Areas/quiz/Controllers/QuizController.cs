using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace text_loginWithBackgrount.Areas.quiz.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student,teacher")]
    [Area("quiz")]
    public class QuizController : Controller
    {

        public IActionResult StdIndex()
        {
            return View();
        }


        public IActionResult Quiz()
        {
            return View();
        }
    }
}
