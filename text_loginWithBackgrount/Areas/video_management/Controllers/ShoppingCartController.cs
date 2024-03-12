using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.video_management.Controllers
{
    public class ShoppingCartController : Controller
    { /// <summary>
      /// 學生登入後會看到的顯示頁面
      /// </summary>
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")]
        [Area("video_management")]
        public IActionResult Cart()
        {
            string userId = User.Claims.FirstOrDefault(p=>p.Type=="StudentId")?.Value;
            return View();
        }
    }
}
