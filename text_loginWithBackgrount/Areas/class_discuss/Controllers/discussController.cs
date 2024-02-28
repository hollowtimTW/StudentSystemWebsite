using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.class_discuss.Controllers
{
    [Area("class_discuss")]
    public class discussController : Controller
    {
        /// <summary>
        /// 訪客能看到的基本分頁
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Announcement()
        {
            return View();
        }
    }
}
