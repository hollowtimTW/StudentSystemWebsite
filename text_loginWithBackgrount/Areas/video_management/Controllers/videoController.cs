using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.video_management.Controllers
{
    [Area("video_management")]
    /// <summary>
    /// 訪客能看到的基本分頁
    /// </summary>
    public class videoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
