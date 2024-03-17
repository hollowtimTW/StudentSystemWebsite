using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.quiz.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("quiz")]
    [Route("quiz/QuizBg/[action]")]
    public class QuizBgController : Controller
    {
        public IActionResult BgIndex()
        {
            return View();
        }

        [HttpGet("{quizId}")]
        public IActionResult BgCreate(int quizId)
        {
            ViewBag.quizId = quizId;
            return View();
        }
    }
}
