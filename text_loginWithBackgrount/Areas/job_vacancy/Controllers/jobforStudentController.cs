using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.job_vacancy.Controllers
{
    /// <summary>
    /// 學生登入後會看到的顯示頁面
    /// </summary>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")]
    [Area("job_vacancy")]
    public class jobforStudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
