using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.class_discuss.Controllers
{
    /// <summary>
    /// 學生登入後會看到的顯示頁面
    /// </summary>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")]
    [Area("class_discuss")]
    public class discussforStudentController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.userId = GetUserId();
            return View();
        }
        private int GetUserId()
        {
            var studentId = User.FindFirst("StudentId")?.Value;
            int userId;
            int.TryParse(studentId, out userId);
            Console.WriteLine(studentId);
            return userId;
        }

    }
}
