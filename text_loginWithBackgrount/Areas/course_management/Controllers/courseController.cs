using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.course_management.Controllers
{
    /// <summary>
    /// 訪客看到的頁面
    /// </summary>
    [Area("course_management")]
    public class courseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
